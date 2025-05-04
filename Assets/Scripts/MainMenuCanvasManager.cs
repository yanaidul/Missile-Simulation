using UnityEngine;

public class MainMenuCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _settingCanvas;

    private void Start()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _settingCanvas.gameObject.SetActive(false);
    }

    public void OnOpenSetting()
    {
        _mainMenuCanvas.gameObject.SetActive(false);
        _settingCanvas.gameObject.SetActive(true);
    }

    public void OnBackToMainMenu()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _settingCanvas.gameObject.SetActive(false);
    }

    
}
