using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MissileSimulation.Missile
{
    public class AimManager : Singleton<AimManager>
    {
        [Header("Script Reference")]
        [SerializeField] private MissileSpawner _missileSpawner;
        [SerializeField] private MissileManager _missileManager;
        [SerializeField] private Timer _timerScript;

        [Header("Crosshair Components")]
        [SerializeField] private Image _crossHair;
        [SerializeField] private Image _followCrossHair;
        [SerializeField] private Image _frontPlaneCrossHair;
        [SerializeField] private Sprite _normalCrosshair;
        [SerializeField] private Sprite _targetedCrosshair;
        [SerializeField] private Sprite _shootCrosshair;

        [Header("Raycast Settings")]
        [SerializeField] private float _raycastDistance = 100f;
        [SerializeField] private Color _rayColor = Color.green;

        [Header("Aimed Target Components")]
        [SerializeField] private CapsuleCollider _frontPlaneCollider;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _frontPlaneTransform;
        [SerializeField] private float _smoothTime = 0.3f;
        [SerializeField] private bool _isLockedOnFront = false;
        [SerializeField] private bool _isLocked = false;
        [SerializeField] private bool _isTargetingPlane = false;
        [SerializeField] private bool _isMissileLaunched = false;

        [Header("Input Action Asset")]
        [SerializeField] private InputActionReference _lockAction;
        [SerializeField] private InputActionReference _shootAction;

        private float _timer = 0f;
        private float _timeLimit = 2f;
        private Camera mainCamera;
        private RectTransform _crossHairRectTransform;
        private RectTransform _frontPlaneCrossHairRectTransform;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _velocity2 = Vector3.zero;
        private Vector3 _targetPosition = Vector3.zero;
        private Vector3 _frontPlaneTargetPosition = Vector3.zero;
        public bool IsLockedOnFront => _isLockedOnFront;

        void OnEnable()
        {
            _lockAction.action.Enable();
        }

        void OnDisable()
        {
            _lockAction.action.Disable();
        }

        protected override void Awake()
        {
            _crossHairRectTransform = _followCrossHair.GetComponent<RectTransform>();
            _frontPlaneCrossHairRectTransform = _frontPlaneCrossHair.GetComponent<RectTransform>();
        }

        void Start()
        {
            _isMissileLaunched = false;
            _frontPlaneCollider.enabled = false;
            _timerScript.gameObject.SetActive(false);
            _followCrossHair.gameObject.SetActive(false);
            _frontPlaneCrossHair.gameObject.SetActive(false);
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (!GameManager.GetInstance().IsGameActive) return;
            if (MissileManager.GetInstance().CurrentMissileNumber == 0) return;

            // Create a ray from the camera going forward
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            // Visualize the ray in the Scene view (for debugging)
            Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, _rayColor);

            if (_isTargetingPlane && !_isLocked)
            {
                StartLockedTimer();
            }


            if (_isLocked)
            {
                TargetLockedOnHandler();
            }

            RaycastCheck(ray, out hit);

            if (_shootAction.action.WasPressedThisFrame() && _isLocked && !_isMissileLaunched)
            {
                if (_target == null)
                {
                    Debug.Log("No target found");
                    return;
                }
                SfxManager.GetInstance().PlayEngageEnemySFX();
                _isMissileLaunched = true;
                _missileManager.DisableCurrentMissile();
                _missileSpawner.OnSpawnMissile(_target.gameObject);
                RestoreCrosshairToInitialState();
            }
        }

        private void TargetLockedOnHandler()
        {
            if (_target == null) return;
            _targetPosition = mainCamera.WorldToScreenPoint(_target.position);
            _frontPlaneTargetPosition = mainCamera.WorldToScreenPoint(_frontPlaneTransform.position);

            Vector3 normalizedTargetPos = mainCamera.ScreenToViewportPoint(_targetPosition);
            Vector3 normalizedFrontPlanePos = mainCamera.ScreenToViewportPoint(_frontPlaneTargetPosition);

            _targetPosition = new Vector3(
                normalizedTargetPos.x * Screen.width,
                normalizedTargetPos.y * Screen.height,
                normalizedTargetPos.z
            );

            _frontPlaneTargetPosition = new Vector3(
                normalizedFrontPlanePos.x * Screen.width,
                normalizedFrontPlanePos.y * Screen.height,
                normalizedFrontPlanePos.z
            );

            _followCrossHair.gameObject.SetActive(true);
            _frontPlaneCrossHair.gameObject.SetActive(true);
            _crossHair.sprite = _shootCrosshair;

            _frontPlaneCrossHairRectTransform.position = Vector3.SmoothDamp(
            _frontPlaneCrossHairRectTransform.position,
            _frontPlaneTargetPosition,
            ref _velocity2,
            _smoothTime);

            _crossHairRectTransform.position = Vector3.SmoothDamp(
            _crossHairRectTransform.position,
            _targetPosition,
            ref _velocity,
            _smoothTime);
        }

        private void RaycastCheck(Ray ray, out RaycastHit hit)
        {
            // Perform the raycast
            if (Physics.Raycast(ray, out hit, _raycastDistance))
            {
                // Check if the hit object has the "Plane" tag
                if (hit.collider.CompareTag("Plane"))
                {
                    if (_lockAction.action.WasPressedThisFrame() && !_isLocked)
                    {
                        _isTargetingPlane = true;
                        _frontPlaneCollider.enabled = true;
                        _crossHair.sprite = _targetedCrosshair;
                        _target = hit.collider.gameObject.transform;
                    }
                }

                if (hit.collider.CompareTag("FrontPlane") && _isLocked && !_isMissileLaunched)
                {
                    _isLockedOnFront = true;
                }
                else if (!_isMissileLaunched && _isLocked) _isLockedOnFront = false;
            }
            else if (!_isMissileLaunched && _isLocked) _isLockedOnFront = false;
            else if (!_isLocked && _isTargetingPlane) RestoreCrosshairToInitialState();
        }

        private void StartLockedTimer()
        {
            _timer += Time.deltaTime;

            if (_timer >= _timeLimit)
            {
                _timerScript.gameObject.SetActive(true);
                _timerScript.StartTimer();
                _isLocked = true;
            }
        }

        public void RestoreCrosshairToInitialState()
        {
            _timerScript.OnStopTimer();
            _timer = 0;
            _frontPlaneCollider.enabled = false;
            _isLocked = false;
            _isTargetingPlane = false;
            _isMissileLaunched = false;
            _crossHair.sprite = _normalCrosshair;
            _followCrossHair.gameObject.SetActive(false);
            _frontPlaneCrossHair.gameObject.SetActive(false);
        }
    } 
}
