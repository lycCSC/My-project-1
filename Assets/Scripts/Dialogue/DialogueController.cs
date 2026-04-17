using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    const string SampleDialogueResourcePath = "Dialogue/SampleBranchDialogue";
    const string SampleNpcName = "DialogueNpcDemo";

    static DialogueController instance;

    readonly List<DialogueTrigger> nearbyTriggers = new();
    DialogueUIController uiController;
    DialogueTrigger activeTrigger;
    DialogueAsset activeDialogue;
    DialogueNodeData activeNode;
    PlayerState cachedPlayerState = PlayerState.Front;
    PlayerStatus cachedPlayerStatus = PlayerStatus.idle;

    public static DialogueController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<DialogueController>();
                if (instance == null)
                {
                    GameObject controllerObject = new("DialogueController");
                    instance = controllerObject.AddComponent<DialogueController>();
                }
            }

            return instance;
        }
    }

    public bool IsDialogueActive => activeDialogue != null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        EnsureSceneBindings(SceneManager.GetActiveScene());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (uiController == null)
        {
            return;
        }

        if (IsDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceDialogue();
            }

            return;
        }

        if (GameplayPauseController.IsPaused)
        {
            uiController.SetInteractionState(false, string.Empty);
            return;
        }

        DialogueTrigger interactableTrigger = GetClosestInteractableTrigger();
        if (interactableTrigger == null)
        {
            uiController.SetInteractionState(false, string.Empty);
            return;
        }

        uiController.SetInteractionState(true, interactableTrigger.interactionPrompt);
        if (Input.GetKeyDown(KeyCode.E))
        {
            BeginDialogue(interactableTrigger);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (IsDialogueActive)
        {
            ForceCloseDialogue();
        }

        EnsureSceneBindings(scene);
    }

    public void RegisterNearbyTrigger(DialogueTrigger trigger)
    {
        if (trigger == null || nearbyTriggers.Contains(trigger))
        {
            return;
        }

        nearbyTriggers.Add(trigger);
    }

    public void UnregisterNearbyTrigger(DialogueTrigger trigger)
    {
        if (trigger == null)
        {
            return;
        }

        nearbyTriggers.Remove(trigger);
    }

    public void TryInteract()
    {
        if (IsDialogueActive || GameplayPauseController.IsPaused)
        {
            return;
        }

        DialogueTrigger interactableTrigger = GetClosestInteractableTrigger();
        if (interactableTrigger != null)
        {
            BeginDialogue(interactableTrigger);
        }
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueActive || activeNode == null)
        {
            return;
        }

        if (activeNode.choices != null && activeNode.choices.Count > 0)
        {
            return;
        }

        if (activeNode.isEndNode || string.IsNullOrWhiteSpace(activeNode.nextNodeId))
        {
            EndDialogue();
            return;
        }

        MoveToNode(activeNode.nextNodeId);
    }

    public void SelectChoice(int index)
    {
        if (!IsDialogueActive || activeNode == null || activeNode.choices == null)
        {
            return;
        }

        if (index < 0 || index >= activeNode.choices.Count)
        {
            return;
        }

        string nextNodeId = activeNode.choices[index].nextNodeId;
        if (string.IsNullOrWhiteSpace(nextNodeId))
        {
            EndDialogue();
            return;
        }

        MoveToNode(nextNodeId);
    }

    void EnsureSceneBindings(Scene scene)
    {
        EnsureUiCreated();
        if (scene.name == "Scene01")
        {
            EnsureSampleNpcExists();
        }
    }

    void EnsureUiCreated()
    {
        if (uiController != null)
        {
            return;
        }

        Canvas canvas = GameObject.Find("C_UserUI")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            GameObject canvasObject = new("DialogueCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        uiController = DialogueUIController.Create(canvas);
        uiController.BindInteractAction(TryInteract);
        uiController.BindContinueAction(AdvanceDialogue);
    }

    void EnsureSampleNpcExists()
    {
        if (GameObject.Find(SampleNpcName) != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(Tags.Player.ToString());
        if (playerObject == null)
        {
            return;
        }

        DialogueAsset sampleDialogue = Resources.Load<DialogueAsset>(SampleDialogueResourcePath);
        if (sampleDialogue == null)
        {
            Debug.LogError($"Sample dialogue asset not found at Resources/{SampleDialogueResourcePath}.");
            return;
        }

        GameObject npcObject = new(SampleNpcName);
        npcObject.transform.position = playerObject.transform.position + new Vector3(3.5f, 0f, 0f);
        BoxCollider2D boxCollider2D = npcObject.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        boxCollider2D.offset = new Vector2(0f, 1f);
        boxCollider2D.size = new Vector2(1.4f, 2.2f);

        DialogueTrigger dialogueTrigger = npcObject.AddComponent<DialogueTrigger>();
        dialogueTrigger.dialogueAsset = sampleDialogue;
        dialogueTrigger.interactionPrompt = "按 E 或点按钮对话";

        BuildSampleNpcVisual(npcObject.transform, playerObject.transform);
    }

    void BuildSampleNpcVisual(Transform npcRoot, Transform playerTransform)
    {
        PlayerProperty playerProperty = FindAnyObjectByType<PlayerProperty>();
        string modelName = playerProperty != null ? playerProperty.modelName : "Devil";

        Transform playerVisualRoot = playerTransform.Find(modelName);
        if (playerVisualRoot == null)
        {
            playerVisualRoot = playerTransform.Find("Devil");
        }

        if (playerVisualRoot == null)
        {
            return;
        }

        GameObject clonedVisual = Instantiate(playerVisualRoot.gameObject, npcRoot);
        clonedVisual.name = "Visual";
        clonedVisual.transform.localPosition = Vector3.zero;
        clonedVisual.transform.localRotation = Quaternion.identity;
        clonedVisual.transform.localScale = Vector3.one;

        for (int i = 0; i < clonedVisual.transform.childCount; i++)
        {
            Transform stateTransform = clonedVisual.transform.GetChild(i);
            stateTransform.gameObject.SetActive(stateTransform.name == PlayerState.Front.ToString());
        }

        Transform frontTransform = clonedVisual.transform.Find(PlayerState.Front.ToString());
        if (frontTransform != null)
        {
            Animator npcAnimator = frontTransform.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.Play("idleFront", 0, 0f);
            }
        }
    }

    DialogueTrigger GetClosestInteractableTrigger()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(Tags.Player.ToString());
        if (playerObject == null)
        {
            return null;
        }

        DialogueTrigger bestTrigger = null;
        float bestDistance = float.MaxValue;

        for (int i = nearbyTriggers.Count - 1; i >= 0; i--)
        {
            DialogueTrigger trigger = nearbyTriggers[i];
            if (trigger == null)
            {
                nearbyTriggers.RemoveAt(i);
                continue;
            }

            if (!trigger.CanStartDialogue())
            {
                continue;
            }

            float distance = Vector2.Distance(playerObject.transform.position, trigger.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTrigger = trigger;
            }
        }

        return bestTrigger;
    }

    void BeginDialogue(DialogueTrigger trigger)
    {
        if (trigger == null || !trigger.CanStartDialogue())
        {
            return;
        }

        if (GameplayPauseController.IsPaused && !GameplayPauseController.IsDialoguePaused)
        {
            return;
        }

        DialogueNodeData startNode = trigger.dialogueAsset.GetStartNode();
        if (startNode == null)
        {
            Debug.LogError($"Dialogue '{trigger.dialogueAsset.name}' has no valid start node.");
            return;
        }

        activeTrigger = trigger;
        activeDialogue = trigger.dialogueAsset;
        activeNode = startNode;
        trigger.MarkTriggered();

        GameplayPauseController.Pause(GameplayPauseReason.Dialogue);
        EnterPlayerTalkState();
        uiController.ShowNode(activeNode, this);
    }

    void MoveToNode(string nodeId)
    {
        DialogueNodeData nextNode = activeDialogue.GetNode(nodeId);
        if (nextNode == null)
        {
            Debug.LogError($"Dialogue node '{nodeId}' was not found in dialogue '{activeDialogue.name}'.");
            EndDialogue();
            return;
        }

        activeNode = nextNode;
        uiController.ShowNode(activeNode, this);
    }

    void EndDialogue()
    {
        activeDialogue = null;
        activeNode = null;
        activeTrigger = null;

        ExitPlayerTalkState();
        GameplayPauseController.Resume(GameplayPauseReason.Dialogue);
        uiController.HideDialogue();
    }

    void ForceCloseDialogue()
    {
        activeDialogue = null;
        activeNode = null;
        activeTrigger = null;
        GameplayPauseController.Resume(GameplayPauseReason.Dialogue);
        if (uiController != null)
        {
            uiController.HideDialogue();
        }
    }

    void EnterPlayerTalkState()
    {
        PlayerProperty playerProperty = FindAnyObjectByType<PlayerProperty>();
        if (playerProperty == null)
        {
            return;
        }

        cachedPlayerState = playerProperty.playerState;
        cachedPlayerStatus = playerProperty.playerStatus;
        playerProperty.ChangeStatus(PlayerStatus.talk);

        Transform modelTransform = playerProperty.transform.Find(playerProperty.modelName);
        if (modelTransform == null)
        {
            modelTransform = playerProperty.transform.Find("Devil");
        }

        if (modelTransform == null)
        {
            return;
        }

        PlayerState talkState = cachedPlayerState == PlayerState.Back ? PlayerState.Back : PlayerState.Front;
        playerProperty.SetAndGetActiveBoneAnimator(talkState, modelTransform);
        if (playerProperty.playerCurrentAnimator == null)
        {
            return;
        }

        playerProperty.playerCurrentAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        string animationName = talkState == PlayerState.Back ? "talkBack" : "talkFront";
        playerProperty.playerCurrentAnimator.Play(animationName, 0, 0f);
    }

    void ExitPlayerTalkState()
    {
        PlayerProperty playerProperty = FindAnyObjectByType<PlayerProperty>();
        if (playerProperty == null)
        {
            return;
        }

        Transform modelTransform = playerProperty.transform.Find(playerProperty.modelName);
        if (modelTransform == null)
        {
            modelTransform = playerProperty.transform.Find("Devil");
        }

        if (playerProperty.playerCurrentAnimator != null)
        {
            playerProperty.playerCurrentAnimator.updateMode = AnimatorUpdateMode.Normal;
        }

        if (modelTransform != null)
        {
            playerProperty.SetAndGetActiveBoneAnimator(cachedPlayerState, modelTransform);
        }

        playerProperty.ChangeStatus(cachedPlayerStatus);
        if (cachedPlayerState == PlayerState.Side && cachedPlayerStatus == PlayerStatus.walk)
        {
            playerProperty.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isWalk);
        }
        else if (cachedPlayerStatus == PlayerStatus.jump)
        {
            if (playerProperty.playerState == PlayerState.Side)
            {
                playerProperty.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isJump);
            }
            else
            {
                playerProperty.ChangeCurrentAnimation(PlayerFrontAnimationTransParam.isJump);
            }
        }
        else if (playerProperty.playerCurrentAnimator != null && cachedPlayerState == PlayerState.Back)
        {
            playerProperty.playerCurrentAnimator.Play("idleBack", 0, 0f);
        }
        else
        {
            playerProperty.ChangeStatus(PlayerStatus.idle);
            if (modelTransform != null)
            {
                playerProperty.SetAndGetActiveBoneAnimator(PlayerState.Front, modelTransform);
            }
            playerProperty.ChangeCurrentAnimation(PlayerFrontAnimationTransParam.isIdle);
        }
    }
}
