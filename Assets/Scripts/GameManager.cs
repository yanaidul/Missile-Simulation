using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private CanvasManager _canvasManager;

    private bool _isGameActive = true;

    public bool IsGameActive => _isGameActive;


    private void Start()
    {
        _isGameActive = true;
    }

    public void SetGameOver(bool isWin)
    {
        _isGameActive = false;
        _canvasManager.SetResultCanvas(isWin);
    }

    public void StartReplay()
    {
        _canvasManager.TurnOnReplayCanvas();
    }
}
