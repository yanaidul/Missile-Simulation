using MissileSimulation.Missile;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameEventNoParam _onLockOnTimeOver;

    public bool stopTimer = false;
    private float _totalDurations;
    private float _beepTimer;
    [SerializeField] private float _countdownTime = 360;

    public float CountdownTime => _countdownTime;

    public void StartTimer()
    {
        stopTimer = false;
        UpdateTimerText();
    }

    private void Start()
    {
        _totalDurations = _countdownTime;
        UpdateTimerText();
    }

    public void OnResetTimer()
    {
        _countdownTime = 45;
        stopTimer = true;
        UpdateTimerText();
    }

    void Update()
    {
        if (stopTimer) return;

        if (_countdownTime > 0)
        {
            _beepTimer += Time.deltaTime;
            if (_beepTimer >= 1) 
            {
                SfxManager.GetInstance().PlayBeepSFX();
                _beepTimer = 0;
            }
            _countdownTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            _countdownTime = 0;
            _onLockOnTimeOver.Raise();
            Debug.Log("Timer expired!");
        }
    }

    public void OnSetStopTimerValue(bool value)
    {
        stopTimer = value;
    }

    public void OnStopTimer()
    {
        OnResetTimer();
        gameObject.SetActive(false);
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(_countdownTime / 60);
        int seconds = Mathf.FloorToInt(_countdownTime % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        _timerText.SetText(timerString);
    }
}