using System.Collections.Generic;
using UnityEngine;

public enum GameplayPauseReason
{
    Menu,
    Dialogue
}

public static class GameplayPauseController
{
    static readonly HashSet<GameplayPauseReason> PauseReasons = new();

    public static bool IsPaused => PauseReasons.Count > 0;
    public static bool IsDialoguePaused => PauseReasons.Contains(GameplayPauseReason.Dialogue);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState()
    {
        PauseReasons.Clear();
        ApplyTimeScale();
    }

    public static void Pause(GameplayPauseReason reason)
    {
        if (!PauseReasons.Add(reason))
        {
            return;
        }

        ApplyTimeScale();
    }

    public static void Resume(GameplayPauseReason reason)
    {
        if (!PauseReasons.Remove(reason))
        {
            return;
        }

        ApplyTimeScale();
    }

    static void ApplyTimeScale()
    {
        Time.timeScale = PauseReasons.Count > 0 ? 0f : 1f;
    }
}
