using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// 画面下中央に「投げる」uGUI ボタンを配置し、BallLauncher に接続する。
/// </summary>
public class ThrowButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button throwButton;

    [Header("Button Layout")]
    [SerializeField] private string buttonLabel = "投げる";
    [SerializeField] private Vector2 anchoredPosition = new Vector2(0f, 96f);
    [SerializeField] private Vector2 buttonSize = new Vector2(260f, 88f);
    [SerializeField] private int fontSize = 42;
    [SerializeField] private Color blockColor = new Color(0.12f, 0.45f, 0.28f, 0.95f);
    [SerializeField] private Color labelColor = Color.white;

    private void Awake()
    {
        if (ballLauncher == null)
        {
            ballLauncher = FindFirstObjectByType<BallLauncher>();
        }

        EnsureEventSystem();

        if (throwButton == null)
        {
            throwButton = BuildThrowButton();
        }

        if (ballLauncher != null && throwButton != null)
        {
            ballLauncher.SetThrowButton(throwButton);
        }
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            if (EventSystem.current.GetComponent<InputSystemUIInputModule>() == null
                && EventSystem.current.GetComponent<BaseInputModule>() == null)
            {
                EventSystem.current.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private Button BuildThrowButton()
    {
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject(
                "ThrowCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject buttonObject = new GameObject(
            "ThrowButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));
        buttonObject.transform.SetParent(canvas.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = buttonSize;

        Image image = buttonObject.GetComponent<Image>();
        image.color = blockColor;
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = blockColor;
        colors.highlightedColor = blockColor * 1.08f;
        colors.pressedColor = blockColor * 0.85f;
        colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
        button.targetGraphic = image;

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObject.GetComponent<Text>();
        label.text = buttonLabel;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (label.font == null)
        {
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = labelColor;
        label.raycastTarget = false;

        return button;
    }
}
