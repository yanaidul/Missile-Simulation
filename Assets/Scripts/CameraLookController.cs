using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MissileSimulation
{
    public class CameraLookController : MonoBehaviour
    {
        [Header("Global Volume")]
        [SerializeField] private Volume _volume;
        [SerializeField] private VolumeProfile _normalVolumeProfile;
        [SerializeField] private VolumeProfile _blackWhiteVolumeProfile;
        [SerializeField] private bool _isBlackWhite;

        [Header("Camera Reference")]
        [SerializeField] private GameObject _camera;
        [SerializeField] private GameObject _replayCamera;

        [Header("Sensitivity & Rotation")]
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _verticalClamp = 80f; // Clamp only vertical rotation
        [SerializeField] private bool _invertY = false;

        [Header("Input Action Asset")]
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference zoomInAction;
        [SerializeField] private InputActionReference zoomOutAction;

        private float _xRotation = 0f;
        private float _yRotation = 0f;
        private Quaternion _initialCameraRotation;

        void OnEnable()
        {
            lookAction.action.Enable();
            _initialCameraRotation = _camera.transform.localRotation;
        }

        void OnDisable()
        {
            lookAction.action.Disable();
        }

        private void Awake()
        {
            if (PlayerPrefs.HasKey("Filter"))
            {
                _isBlackWhite = PlayerPrefs.GetInt("Filter") == 1;
            }

            _volume.profile = _isBlackWhite ? _blackWhiteVolumeProfile : _normalVolumeProfile;
        }

        void Update()
        {
            if (_camera == null || !GameManager.GetInstance().IsGameActive) return;

            HandleZoomInput();
            HandleCameraRotation();
        }

        private void HandleZoomInput()
        {
            if (zoomInAction.action.WasPressedThisFrame())
            {
                SetFieldOfView(30);
            }
            else if (zoomOutAction.action.WasPressedThisFrame())
            {
                SetFieldOfView(60);
            }
        }

        private void SetFieldOfView(float fov)
        {
            if (_camera.TryGetComponent(out Camera camera))
            {
                camera.fieldOfView = fov;
            }

            if (_replayCamera.TryGetComponent(out Camera replayCamera))
            {
                replayCamera.fieldOfView = fov;
            }
        }

        private void HandleCameraRotation()
        {
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            float mouseX = lookInput.x * _sensitivity * Time.deltaTime;
            float mouseY = lookInput.y * _sensitivity * Time.deltaTime * (_invertY ? 1 : -1);

            // Unclamped horizontal rotation (full 360 degrees)
            _yRotation += mouseX;

            // Clamped vertical rotation
            _xRotation += mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_verticalClamp, _verticalClamp);

            // Apply rotation to cameras
            Quaternion targetRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _camera.transform.localRotation = _initialCameraRotation * targetRotation;
            _replayCamera.transform.localRotation = _initialCameraRotation * targetRotation;
        }
    }
}
