using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    static Sprite runtimeSprite;

    RectTransform rootTransform;
    GameObject promptRoot;
    TextMeshProUGUI promptText;
    Button interactButton;
    TextMeshProUGUI interactButtonText;

    GameObject dialogueRoot;
    TextMeshProUGUI speakerText;
    TextMeshProUGUI bodyText;
    Button continueButton;
    TextMeshProUGUI continueButtonText;
    RectTransform choiceContainer;
    readonly List<GameObject> spawnedChoiceButtons = new();

    public static DialogueUIController Create(Canvas parentCanvas)
    {
        GameObject root = new("DialogueRuntimeUI", typeof(RectTransform));
        root.transform.SetParent(parentCanvas.transform, false);
        DialogueUIController controller = root.AddComponent<DialogueUIController>();
        controller.Build();
        return controller;
    }

    void Build()
    {
        rootTransform = GetComponent<RectTransform>();
        rootTransform.anchorMin = Vector2.zero;
        rootTransform.anchorMax = Vector2.one;
        rootTransform.offsetMin = Vector2.zero;
        rootTransform.offsetMax = Vector2.zero;

        promptRoot = CreatePanel("InteractPrompt", rootTransform, new Color(0f, 0f, 0f, 0.7f));
        RectTransform promptRect = promptRoot.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 1f);
        promptRect.anchorMax = new Vector2(0.5f, 1f);
        promptRect.pivot = new Vector2(0.5f, 1f);
        promptRect.sizeDelta = new Vector2(360f, 56f);
        promptRect.anchoredPosition = new Vector2(0f, -32f);
        promptText = CreateText("PromptText", promptRect, 26, FontStyles.Bold);
        promptText.alignment = TextAlignmentOptions.Center;
        StretchToParent(promptText.rectTransform, new Vector2(18f, 8f), new Vector2(-18f, -8f));

        interactButton = CreateButton("InteractButton", rootTransform, "对话");
        RectTransform interactRect = interactButton.GetComponent<RectTransform>();
        interactRect.anchorMin = new Vector2(1f, 0f);
        interactRect.anchorMax = new Vector2(1f, 0f);
        interactRect.pivot = new Vector2(1f, 0f);
        interactRect.sizeDelta = new Vector2(160f, 68f);
        interactRect.anchoredPosition = new Vector2(-140f, 200f);
        interactButtonText = interactButton.GetComponentInChildren<TextMeshProUGUI>();

        dialogueRoot = CreatePanel("DialoguePanel", rootTransform, new Color(0.05f, 0.07f, 0.12f, 0.95f));
        RectTransform dialogueRect = dialogueRoot.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0.08f, 0.04f);
        dialogueRect.anchorMax = new Vector2(0.92f, 0.34f);
        dialogueRect.offsetMin = Vector2.zero;
        dialogueRect.offsetMax = Vector2.zero;

        speakerText = CreateText("SpeakerText", dialogueRect, 30, FontStyles.Bold);
        speakerText.alignment = TextAlignmentOptions.TopLeft;
        speakerText.rectTransform.anchorMin = new Vector2(0f, 1f);
        speakerText.rectTransform.anchorMax = new Vector2(1f, 1f);
        speakerText.rectTransform.pivot = new Vector2(0f, 1f);
        speakerText.rectTransform.sizeDelta = new Vector2(0f, 40f);
        speakerText.rectTransform.anchoredPosition = new Vector2(32f, -24f);

        bodyText = CreateText("BodyText", dialogueRect, 28, FontStyles.Normal);
        bodyText.enableWordWrapping = true;
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
        bodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
        bodyText.rectTransform.offsetMin = new Vector2(32f, 88f);
        bodyText.rectTransform.offsetMax = new Vector2(-32f, -72f);

        GameObject choicesRoot = new("Choices", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        choicesRoot.transform.SetParent(dialogueRect, false);
        choiceContainer = choicesRoot.GetComponent<RectTransform>();
        choiceContainer.anchorMin = new Vector2(0f, 0f);
        choiceContainer.anchorMax = new Vector2(1f, 0f);
        choiceContainer.pivot = new Vector2(0.5f, 0f);
        choiceContainer.anchoredPosition = new Vector2(0f, 18f);
        choiceContainer.sizeDelta = new Vector2(-64f, 10f);
        VerticalLayoutGroup layoutGroup = choicesRoot.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(0, 0, 0, 0);
        layoutGroup.spacing = 12f;
        layoutGroup.childAlignment = TextAnchor.LowerCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        ContentSizeFitter fitter = choicesRoot.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        continueButton = CreateButton("ContinueButton", dialogueRect, "继续");
        RectTransform continueRect = continueButton.GetComponent<RectTransform>();
        continueRect.anchorMin = new Vector2(1f, 0f);
        continueRect.anchorMax = new Vector2(1f, 0f);
        continueRect.pivot = new Vector2(1f, 0f);
        continueRect.sizeDelta = new Vector2(148f, 56f);
        continueRect.anchoredPosition = new Vector2(-24f, 18f);
        continueButtonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();

        HideDialogue();
        SetInteractionState(false, string.Empty);
    }

    public void BindInteractAction(UnityEngine.Events.UnityAction action)
    {
        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(action);
    }

    public void BindContinueAction(UnityEngine.Events.UnityAction action)
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(action);
    }

    public void SetInteractionState(bool canInteract, string prompt)
    {
        promptRoot.SetActive(canInteract);
        interactButton.gameObject.SetActive(canInteract);
        promptText.text = canInteract ? prompt : string.Empty;
        interactButtonText.text = "对话";
    }

    public void ShowDialogue()
    {
        dialogueRoot.SetActive(true);
        promptRoot.SetActive(false);
        interactButton.gameObject.SetActive(false);
    }

    public void HideDialogue()
    {
        dialogueRoot.SetActive(false);
        ClearChoices();
    }

    public void ShowNode(DialogueNodeData nodeData, DialogueController controller)
    {
        ShowDialogue();
        ClearChoices();

        speakerText.text = string.IsNullOrWhiteSpace(nodeData.speakerName) ? " " : nodeData.speakerName;
        bodyText.text = nodeData.message;

        bool hasChoices = nodeData.choices != null && nodeData.choices.Count > 0;
        continueButton.gameObject.SetActive(!hasChoices);
        continueButtonText.text = nodeData.isEndNode ? "结束" : "继续";

        if (!hasChoices)
        {
            return;
        }

        for (int i = 0; i < nodeData.choices.Count; i++)
        {
            int choiceIndex = i;
            Button choiceButton = CreateButton($"Choice_{choiceIndex}", choiceContainer, nodeData.choices[choiceIndex].choiceText);
            choiceButton.onClick.AddListener(() => controller.SelectChoice(choiceIndex));
            RectTransform choiceRect = choiceButton.GetComponent<RectTransform>();
            choiceRect.sizeDelta = new Vector2(0f, 52f);
            spawnedChoiceButtons.Add(choiceButton.gameObject);
        }
    }

    void ClearChoices()
    {
        for (int i = 0; i < spawnedChoiceButtons.Count; i++)
        {
            if (spawnedChoiceButtons[i] != null)
            {
                Destroy(spawnedChoiceButtons[i]);
            }
        }

        spawnedChoiceButtons.Clear();
    }

    static GameObject CreatePanel(string objectName, Transform parent, Color color)
    {
        GameObject panel = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        Image image = panel.GetComponent<Image>();
        image.sprite = GetRuntimeSprite();
        image.type = Image.Type.Simple;
        image.color = color;
        return panel;
    }

    static Button CreateButton(string objectName, Transform parent, string label)
    {
        GameObject buttonObject = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.GetComponent<Image>();
        image.sprite = GetRuntimeSprite();
        image.type = Image.Type.Simple;
        image.color = new Color(0.17f, 0.35f, 0.72f, 0.96f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.22f, 0.42f, 0.82f, 0.98f);
        colors.pressedColor = new Color(0.12f, 0.24f, 0.52f, 0.98f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.3f, 0.3f, 0.35f, 0.6f);
        button.colors = colors;

        TextMeshProUGUI text = CreateText("Label", buttonObject.transform, 24, FontStyles.Bold);
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        StretchToParent(text.rectTransform, new Vector2(12f, 6f), new Vector2(-12f, -6f));

        return button;
    }

    static TextMeshProUGUI CreateText(string objectName, Transform parent, float fontSize, FontStyles fontStyle)
    {
        GameObject textObject = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset != null
            ? TMP_Settings.defaultFontAsset
            : Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = Color.white;
        text.text = string.Empty;
        return text;
    }

    static void StretchToParent(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }

    static Sprite GetRuntimeSprite()
    {
        if (runtimeSprite != null)
        {
            return runtimeSprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        runtimeSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        return runtimeSprite;
    }
}
