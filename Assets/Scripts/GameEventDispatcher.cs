public static class GameEventDispatcher
{
    //Event Setup
    public delegate void GameEventHandler();
    public static event GameEventHandler EnemyDefeated;
    public static event GameEventHandler EnemiesAllDefeated;
    public static event GameEventHandler SlimeCollected;
    public static event GameEventHandler SceneExited;
    public static event GameEventHandler GamePaused;
    public static event GameEventHandler GameResumed;
    public static event GameEventHandler PlayerDefeated;
    
    
    public static bool isGamePaused { get; private set; }
    
    
    // Great description
    // of pausing and how to deal with it:
    // https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
    public static void TriggerGamePaused()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            GamePaused?.Invoke();
        }
    }

    public static void TriggerGameResumed()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            GameResumed?.Invoke();
        }
    }

    public static void TriggerPlayerDefeated()
    {
        PlayerDefeated?.Invoke();
    }

    public static void TriggerEnemyDefeated()
    {
        EnemyDefeated?.Invoke();
    }

    public static void TriggerEnemiesAllDefeated()
    {
        EnemiesAllDefeated?.Invoke();
    }

    public static void TriggerSlimeCollected()
    {
        SlimeCollected?.Invoke();
    }

    public static void TriggerSceneExited()
    {
        SceneExited?.Invoke();
    }
}