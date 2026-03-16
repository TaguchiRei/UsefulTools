using System;
using UnityEngine;

[Serializable]
public class TimeLimitRule : IUpdateRule
{
    public GameState State { get; private set; }
    public event Action<GameState> OnGameEndAction;

    [SerializeField] private float _timiLimit;

    private bool _isPaused;
    private bool _isStopped;
    private float _time;

    public void StartGame()
    {
        State = GameState.Playing;
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
            OnGameEndAction?.Invoke(State);
        }
    }
}