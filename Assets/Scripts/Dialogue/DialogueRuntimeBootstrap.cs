using UnityEngine;

public static class DialogueRuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        _ = DialogueController.Instance;
    }
}
