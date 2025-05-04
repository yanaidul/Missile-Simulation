using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameEventNoParamListener : MonoBehaviour
{
    [SerializeField] private GameEventNoParam _gameEventNoParam;

    [SerializeField] private UnityEvent _responsesNoParam;

    private void OnEnable()
    {
        _gameEventNoParam.RegisterListener(this);
    }

    private void OnDisable()
    {
        _gameEventNoParam.UnregisterListener(this);
    }

    public void OnEventRaisedNoParam()
    {
        _responsesNoParam.Invoke();
    }

    public UnityEvent GetEvent()
    {
        return _responsesNoParam;
    }

    public void SetGameEvent(GameEventNoParam gameEvent)
    {
        _gameEventNoParam = gameEvent;
    }

}
