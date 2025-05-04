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

        [Header("Sensitivity & Clamp")]
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _clampX = 80f;
        [SerializeField] private float _clampY = 80f;

        [Header("Input Action Asset")]
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference zoomInAction;
        [SerializeField] private InputActionReference zoomOutAction;

        private float _xRotation = 0f;
        private float _yRotation = 0f;

        void OnEnable()
        {
            lookAction.action.Enable();
        }

        void OnDisable()
        {
            lookAction.action.Disable();
        }

        private void Awake()
        {
            if (PlayerPrefs.HasKey("Filter"))
            {
                if (PlayerPrefs.GetInt("Filter") == 0) _isBlackWhite = false;
                else _isBlackWhite = true;
            }

            if (_isBlackWhite)
            {
                _volume.profile = _blackWhiteVolumeProfile; 
            }
            else
            {
                _volume.profile = _normalVolumeProfile;
            }
        }

        void Update()
        {
            if (_camera == null) return;
            if (!GameManager.GetInstance().IsGameActive) return;

            if (zoomInAction.action.IsPressed())
            {
                if(_camera.TryGetComponent(out Camera camera))
                {
                    camera.fieldOfView -= (_sensitivity * Time.deltaTime);
                    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 30, 60);
                }

                if (_replayCamera.TryGetComponent(out Camera replayCamera))
                {
                    replayCamera.fieldOfView -= (_sensitivity * Time.deltaTime);
                    replayCamera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 30, 60);
                }
            }

            if (zoomOutAction.action.IsPressed())
            {
                if (_camera.TryGetComponent(out Camera camera))
                {
                    camera.fieldOfView += (_sensitivity * Time.deltaTime);
                    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 30, 60);
                }

                if (_replayCamera.TryGetComponent(out Camera replayCamera))
                {
                    replayCamera.fieldOfView += (_sensitivity * Time.deltaTime);
                    replayCamera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 30, 60);
                }

            }


            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            float mouseX = lookInput.x * _sensitivity * Time.deltaTime;
            float mouseY = lookInput.y * _sensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_clampX, _clampX);

            _yRotation += mouseX;
            _yRotation = Mathf.Clamp(_yRotation, -_clampY, _clampY);

            _camera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _replayCamera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }
    } 
}
