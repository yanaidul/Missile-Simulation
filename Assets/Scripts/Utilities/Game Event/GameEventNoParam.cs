using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEventNoParam")]
public class GameEventNoParam : ScriptableObject
{
    public List<GameEventNoParamListener> listeners = new List<GameEventNoParamListener>();

    public void Raise()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventRaisedNoParam();
        }
    }

    public void RegisterListener(GameEventNoParamListener listener)
    {
        if (listeners.Contains(listener)) return;
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventNoParamListener listener)
    {
        if (!listeners.Contains(listener)) return;
        listeners.Remove(listener);
    }
}
