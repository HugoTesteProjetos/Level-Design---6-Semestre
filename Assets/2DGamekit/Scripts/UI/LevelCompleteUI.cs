using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gamekit2D
{
    [DisallowMultipleComponent]
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("Navigation")]
        public string menuSceneName = "Start";

        [Header("Asset Style")]
        public TMP_FontAsset fontAsset;
        public Color accentColor = new Color(0.98f, 0.024f, 0.376f, 1f);

        GameObject m_CanvasRoot;
        TMP_Text m_Title;
        Button m_RestartButton;
        bool m_PausedGame;

        void Awake()
        {
            BuildInterface();
        }

        public void Show(string title, bool pauseGame)
        {
            BuildInterface();
            m_Title.text = title;
            m_CanvasRoot.SetActive(true);
            m_PausedGame = pauseGame;

            if (pauseGame)
                Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(m_RestartButton.gameObject);
        }

        public void Hide()
        {
            if (m_CanvasRoot != null)
                m_CanvasRoot.SetActive(false);

            if (m_PausedGame)
                Time.timeScale = 1f;

            m_PausedGame = false;
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitToMenu()
        {
            Time.timeScale = 1f;

            if (Application.CanStreamedLevelBeLoaded(menuSceneName))
                SceneManager.LoadScene(menuSceneName);
            else
                Debug.LogError($"Menu scene '{menuSceneName}' is not available in Build Settings.");
        }

        void OnDestroy()
        {
            if (m_PausedGame)
                Time.timeScale = 1f;
        }

        void BuildInterface()
        {
            if (m_CanvasRoot != null)
                return;

            m_CanvasRoot = new GameObject("LevelCompleteCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            m_CanvasRoot.transform.SetParent(transform, false);

            Canvas canvas = m_CanvasRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = m_CanvasRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            Image fade = CreateImage("Fade", m_CanvasRoot.transform, new Color(0.025f, 0.025f, 0.04f, 0.88f));
            Stretch(fade.rectTransform);

            Image panel = CreateImage("ResultPanel", fade.transform, new Color(0.075f, 0.075f, 0.1f, 0.98f));
            SetCenteredSize(panel.rectTransform, new Vector2(760f, 390f));

            Image accent = CreateImage("Accent", panel.transform, accentColor);
            RectTransform accentRect = accent.rectTransform;
            accentRect.anchorMin = new Vector2(0f, 1f);
            accentRect.anchorMax = Vector2.one;
            accentRect.pivot = new Vector2(0.5f, 1f);
            accentRect.sizeDelta = new Vector2(0f, 9f);
            accentRect.anchoredPosition = Vector2.zero;

            m_Title = CreateText("Title", panel.transform, "FASE CONCLUÍDA!", 58f, FontStyles.Bold);
            RectTransform titleRect = m_Title.rectTransform;
            titleRect.anchorMin = new Vector2(0.08f, 0.56f);
            titleRect.anchorMax = new Vector2(0.92f, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            m_RestartButton = CreateButton("RestartButton", panel.transform, "REINICIAR", new Vector2(-175f, -92f));
            m_RestartButton.onClick.AddListener(RestartLevel);

            Button exitButton = CreateButton("ExitButton", panel.transform, "SAIR", new Vector2(175f, -92f));
            exitButton.onClick.AddListener(ExitToMenu);

            m_CanvasRoot.SetActive(false);
        }

        Image CreateImage(string objectName, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        TMP_Text CreateText(string objectName, Transform parent, string value, float size, FontStyles style)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.font = fontAsset != null ? fontAsset : TMP_Settings.defaultFontAsset;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            return text;
        }

        Button CreateButton(string objectName, Transform parent, string label, Vector2 position)
        {
            Image background = CreateImage(objectName, parent, new Color(0.16f, 0.16f, 0.2f, 1f));
            RectTransform rect = background.rectTransform;
            SetCenteredSize(rect, new Vector2(280f, 76f));
            rect.anchoredPosition = position;

            Button button = background.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.16f, 0.16f, 0.2f, 1f);
            colors.highlightedColor = accentColor;
            colors.selectedColor = accentColor;
            colors.pressedColor = new Color(accentColor.r * 0.75f, accentColor.g * 0.75f,
                accentColor.b * 0.75f, 1f);
            button.colors = colors;

            TMP_Text buttonText = CreateText("Text", background.transform, label, 34f, FontStyles.Bold);
            Stretch(buttonText.rectTransform);
            return button;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void SetCenteredSize(RectTransform rect, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
