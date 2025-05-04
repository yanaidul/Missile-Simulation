using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MissileSimulation.Missile
{
    public class MissileManager : Singleton<MissileManager>
    {
        [Header("Missile Button Reference")]
        [SerializeField] private Button _missile1;
        [SerializeField] private Button _missile2;
        [SerializeField] private GameObject _selectedMissile1;
        [SerializeField] private GameObject _selectedMissile2;

        [Header("Input Action Asset")]
        [SerializeField] private InputActionReference _missile1Action;
        [SerializeField] private InputActionReference _missile2Action;

        private int _currentMissileNumber = 0;
        private bool _isMissile1CanBeUsed = true;
        private bool _isMissile2CanBeUsed = true;
        public int CurrentMissileNumber => _currentMissileNumber;
        public bool IsMissile1CanBeUsed => _isMissile1CanBeUsed;
        public bool IsMissile2CanBeUsed => _isMissile2CanBeUsed;


        private void Start()
        {
            OnResetMissileAfterUsed();
            _isMissile1CanBeUsed = true;
            _isMissile2CanBeUsed = true;
        }

        private void Update()
        {
            if (_missile1Action.action.WasPressedThisFrame() && _isMissile1CanBeUsed)
            {
                OnSetActiveMissile1();
            }

            if (_missile2Action.action.WasPressedThisFrame() && _isMissile2CanBeUsed)
            {
                OnSetActiveMissile2();
            }
        }

        private void OnResetMissileAfterUsed()
        {
            _selectedMissile1.gameObject.SetActive(false);
            _selectedMissile2.gameObject.SetActive(false);
            _currentMissileNumber = 0;

        }

        public void OnSetActiveMissile1()
        {
            _selectedMissile1.gameObject.SetActive(true);
            _selectedMissile2.gameObject.SetActive(false);
            _currentMissileNumber = 1;
        }

        public void OnSetActiveMissile2()
        {
            _selectedMissile1.gameObject.SetActive(false);
            _selectedMissile2.gameObject.SetActive(true);
            _currentMissileNumber = 2;
        }

        public void DisableCurrentMissile()
        {
            switch (_currentMissileNumber)
            {
                case 1:
                    _isMissile1CanBeUsed = false;
                    _missile1.interactable = false;
                    _missile1.transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    _isMissile2CanBeUsed = false;
                    _missile2.interactable = false;
                    _missile2.transform.GetChild(1).gameObject.SetActive(true);
                    break;
                default:
                    Debug.Log("No missile selected");
                    break;
            }

            OnResetMissileAfterUsed();
        }
    }
}

