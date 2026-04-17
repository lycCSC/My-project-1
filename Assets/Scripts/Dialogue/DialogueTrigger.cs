using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    public DialogueAsset dialogueAsset;
    public string interactionPrompt = "按 E 对话";
    public bool canRepeat = true;

    bool hasTriggered;

    void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Tags.Player.ToString()))
        {
            return;
        }

        DialogueController.Instance.RegisterNearbyTrigger(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(Tags.Player.ToString()))
        {
            return;
        }

        DialogueController.Instance.UnregisterNearbyTrigger(this);
    }

    public bool CanStartDialogue()
    {
        if (dialogueAsset == null)
        {
            return false;
        }

        return canRepeat || !hasTriggered;
    }

    public void MarkTriggered()
    {
        hasTriggered = true;
    }
}
