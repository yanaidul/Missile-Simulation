using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Toggle _sfxToggle;
    [SerializeField] private Toggle _filterToggle;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetInt("SFX", 0);
        }

        if (!PlayerPrefs.HasKey("Filter"))
        {
            PlayerPrefs.SetInt("Filter", 0);
        }

        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetInt("SFX") == 0) _sfxToggle.isOn = false;
            else _sfxToggle.isOn = true;
        }

        if (PlayerPrefs.HasKey("Filter"))
        {
            if (PlayerPrefs.GetInt("Filter") == 0) _filterToggle.isOn = false;
            else _filterToggle.isOn = true;
        }
    }

    public void OnSFXToggleValueChanged(Toggle change)
    {
        if (change.isOn) 
        {
            PlayerPrefs.SetInt("SFX", 1);
        }
        else PlayerPrefs.SetInt("SFX", 0);
    }

    public void OnFilterToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            PlayerPrefs.SetInt("Filter", 1);
        }
        else PlayerPrefs.SetInt("Filter", 0);
    }
}
