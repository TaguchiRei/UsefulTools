using System;
using UnityEngine;

[Serializable]
public class TimeLimitRule : IUpdateRule
{
    public RuleState State { get; private set; }
    public event Action<RuleState> OnGameEndAction;

    [SerializeField] private float _timiLimit;

    private bool _isPaused;
    private bool _isStopped;
    private float _time;

    public void StartGame()
    {
        State = RuleState.Playing;
        _isStopped = false;
        _isStopped = false;
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    public void Stop()
    {
        _isStopped = true;
    }

    public void Update()
    {
        if (_isStopped || _isPaused) return;
        _time += Time.deltaTime;
        if (_time >= _timiLimit)
        {
            Stop();
            if (OnGameEndAction != null) OnGameEndAction.Invoke(State);
            OnGameEndAction?.Invoke(State);
        }
    }
}