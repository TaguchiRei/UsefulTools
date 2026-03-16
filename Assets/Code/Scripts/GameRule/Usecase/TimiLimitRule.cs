using System;
using System.Threading;
using Cysharp.Threading.Tasks;

[Serializable]
public class TimiLimitRule : IRule
{
    public GameState State { get; private set; }
    public event Action<GameState> OnGameEndAction;

    private CancellationTokenSource _cts;

    public void StartGame()
    {
        State = GameState.Playing;
        _cts = new CancellationTokenSource();
    }

    public void Pause()
    {
    }

    public void Resume()
    {
    }

    public void Stop()
    {
    }
}