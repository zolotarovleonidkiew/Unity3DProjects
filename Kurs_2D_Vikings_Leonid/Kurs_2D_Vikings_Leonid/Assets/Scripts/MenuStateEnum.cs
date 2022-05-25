using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Types of menu in this game
    /// </summary>
    [Serializable]
    public enum MenuStateEnum
    {
        StartGame,
        FailedGame,
        PauseGame,
        WinTheGame
    }
}
