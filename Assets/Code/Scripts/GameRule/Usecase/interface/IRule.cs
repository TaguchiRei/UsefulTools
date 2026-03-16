using System;

public interface IRule
{
    GameState State { get; }
    event Action<GameState> OnGameEndAction;

    /// <summary> ゲームをポーズするときに使用 </summary>
    public void Pause();

    /// <summary> ポーズ解除に使用 </summary>
    public void Resume();

    /// <summary> このルールを停止するときに使用 </summary>
    public void Stop();
}


public enum GameState
{
    Playing,
    GameOver,
    GameClear
}