using System;

[Serializable]
public class TimiLimitRule : IRule
{
    public GameState State { get; private set; }
    public event Action<GameState> OnGameEndAction;

    public void StartGame()
    {
        State = GameState.Playing;
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