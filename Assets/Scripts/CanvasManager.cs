using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _replayCanvas;
    [SerializeField] private GameObject _winCanvas;
    [SerializeField] private GameObject _loseCanvas;
    private void Start()
    {
        _winCanvas.SetActive(false);
        _loseCanvas.SetActive(false);
        _replayCanvas.SetActive(false);
    }

    public void TurnOnReplayCanvas()
    {
        _replayCanvas.SetActive(true);
    }

    public void SetResultCanvas(bool isWin)
    {
        if (isWin)
        {
            _winCanvas.SetActive(true);
            _loseCanvas.SetActive(false);
        }
        else
        {
            _winCanvas.SetActive(false);
            _loseCanvas.SetActive(true);
        }
    }
}
