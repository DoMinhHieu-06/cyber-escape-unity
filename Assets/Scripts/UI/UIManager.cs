using UnityEngine;
using UnityEngine.UI;
using CyberEscape.Core;

namespace CyberEscape.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highScoreText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text shieldStatusText;
        [SerializeField] private Text magnetStatusText;

        [Header("Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("GameOver Elements")]
        [SerializeField] private Text finalScoreText;
        [SerializeField] private Text finalHighScoreText;
        [SerializeField] private Button restartButton;

        [Header("Pause Elements")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseRestartButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Auto-create UI if not assigned
            if (hudPanel == null)
            {
                BuildRuntimeUI();
            }
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreUpdated += UpdateScoreUI;
                GameManager.Instance.OnCoinsUpdated += UpdateCoinsUI;
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                GameManager.Instance.OnShieldChanged += HandleShieldUI;
                GameManager.Instance.OnMagnetTimeRemaining += HandleMagnetUI;

                if (highScoreText != null)
                {
                    highScoreText.text = $"HI-SCORE: {GameManager.Instance.HighScore}";
                }
            }

            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
            if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(OnRestartClicked);

            ShowPanel(hudPanel);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                if (speedText != null)
                {
                    speedText.text = $"SPEED: {GameManager.Instance.CurrentSpeed:F1} m/s";
                }
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreUpdated -= UpdateScoreUI;
                GameManager.Instance.OnCoinsUpdated -= UpdateCoinsUI;
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
                GameManager.Instance.OnShieldChanged -= HandleShieldUI;
                GameManager.Instance.OnMagnetTimeRemaining -= HandleMagnetUI;
            }
        }

        private void UpdateScoreUI(int score)
        {
            if (scoreText != null) scoreText.text = $"SCORE: {score:D6}";
        }

        private void UpdateCoinsUI(int coins)
        {
            if (coinsText != null) coinsText.text = $"COINS: {coins}";
        }

        private void HandleShieldUI(bool active)
        {
            if (shieldStatusText != null)
            {
                shieldStatusText.gameObject.SetActive(active);
                shieldStatusText.text = active ? "[SHIELD ACTIVE]" : "";
            }
        }

        private void HandleMagnetUI(float timeRemaining)
        {
            if (magnetStatusText != null)
            {
                bool active = timeRemaining > 0f;
                magnetStatusText.gameObject.SetActive(active);
                if (active) magnetStatusText.text = $"MAGNET: {timeRemaining:F1}s";
            }
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    if (gameOverPanel != null) gameOverPanel.SetActive(false);
                    if (pausePanel != null) pausePanel.SetActive(false);
                    if (hudPanel != null) hudPanel.SetActive(true);
                    break;

                case GameState.Paused:
                    if (pausePanel != null) pausePanel.SetActive(true);
                    break;

                case GameState.GameOver:
                    if (hudPanel != null) hudPanel.SetActive(false);
                    if (gameOverPanel != null)
                    {
                        gameOverPanel.SetActive(true);
                        if (finalScoreText != null) finalScoreText.text = $"FINAL SCORE\n{GameManager.Instance.CurrentScore}";
                        if (finalHighScoreText != null) finalHighScoreText.text = $"BEST RECORD: {GameManager.Instance.HighScore}";
                    }
                    break;
            }
        }

        private void OnRestartClicked()
        {
            GameManager.Instance?.RestartGame();
        }

        private void OnResumeClicked()
        {
            GameManager.Instance?.TogglePause();
        }

        private void ShowPanel(GameObject panel)
        {
            if (panel != null) panel.SetActive(true);
        }

        /// <summary>
        /// Creates a clean procedural Canvas HUD if no UI canvas is present in the scene.
        /// </summary>
        public void BuildRuntimeUI()
        {
            // Canvas
            GameObject canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // --- HUD PANEL ---
            hudPanel = new GameObject("HUDPanel");
            hudPanel.transform.SetParent(canvasObj.transform, false);
            RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.sizeDelta = Vector2.zero;

            scoreText = CreateText(hudPanel.transform, "ScoreText", "SCORE: 000000", 36, new Vector2(40, -40), new Vector2(0, 1), new Vector2(0, 1), Color.cyan, defaultFont);
            highScoreText = CreateText(hudPanel.transform, "HighScoreText", "HI-SCORE: 0", 24, new Vector2(40, -90), new Vector2(0, 1), new Vector2(0, 1), Color.gray, defaultFont);
            coinsText = CreateText(hudPanel.transform, "CoinsText", "COINS: 0", 32, new Vector2(-40, -40), new Vector2(1, 1), new Vector2(1, 1), Color.yellow, defaultFont);
            coinsText.alignment = TextAnchor.UpperRight;

            speedText = CreateText(hudPanel.transform, "SpeedText", "SPEED: 12.0 m/s", 26, new Vector2(-40, -90), new Vector2(1, 1), new Vector2(1, 1), Color.white, defaultFont);
            speedText.alignment = TextAnchor.UpperRight;

            shieldStatusText = CreateText(hudPanel.transform, "ShieldText", "[SHIELD ACTIVE]", 28, new Vector2(0, -50), new Vector2(0.5f, 1), new Vector2(0.5f, 1), Color.cyan, defaultFont);
            shieldStatusText.alignment = TextAnchor.UpperCenter;
            shieldStatusText.gameObject.SetActive(false);

            magnetStatusText = CreateText(hudPanel.transform, "MagnetText", "MAGNET: 8.0s", 28, new Vector2(0, -90), new Vector2(0.5f, 1), new Vector2(0.5f, 1), Color.green, defaultFont);
            magnetStatusText.alignment = TextAnchor.UpperCenter;
            magnetStatusText.gameObject.SetActive(false);

            // Controls Hint at bottom
            Text controlsHint = CreateText(hudPanel.transform, "HintText", "[A / D / Left / Right] Đổi làn    |    [W / Space] Nhảy    |    [S / Down] Trượt    |    [ESC / P] Tạm dừng", 20, new Vector2(0, 30), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Color(1f, 1f, 1f, 0.7f), defaultFont);
            controlsHint.alignment = TextAnchor.LowerCenter;

            // --- GAME OVER PANEL ---
            gameOverPanel = new GameObject("GameOverPanel");
            gameOverPanel.transform.SetParent(canvasObj.transform, false);
            RectTransform goRect = gameOverPanel.AddComponent<RectTransform>();
            goRect.anchorMin = Vector2.zero;
            goRect.anchorMax = Vector2.one;
            goRect.sizeDelta = Vector2.zero;

            Image bg = gameOverPanel.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

            Text goTitle = CreateText(gameOverPanel.transform, "Title", "GAME OVER", 64, new Vector2(0, 160), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Color.red, defaultFont);
            goTitle.alignment = TextAnchor.MiddleCenter;

            finalScoreText = CreateText(gameOverPanel.transform, "FinalScore", "FINAL SCORE\n0", 42, new Vector2(0, 50), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Color.white, defaultFont);
            finalScoreText.alignment = TextAnchor.MiddleCenter;

            finalHighScoreText = CreateText(gameOverPanel.transform, "BestScore", "BEST RECORD: 0", 28, new Vector2(0, -30), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Color.yellow, defaultFont);
            finalHighScoreText.alignment = TextAnchor.MiddleCenter;

            restartButton = CreateButton(gameOverPanel.transform, "RestartButton", "CHƠI LẠI (RESTART)", new Vector2(0, -120), new Vector2(280, 60), new Color(0.1f, 0.6f, 0.9f), defaultFont);

            gameOverPanel.SetActive(false);

            // --- PAUSE PANEL ---
            pausePanel = new GameObject("PausePanel");
            pausePanel.transform.SetParent(canvasObj.transform, false);
            RectTransform pRect = pausePanel.AddComponent<RectTransform>();
            pRect.anchorMin = Vector2.zero;
            pRect.anchorMax = Vector2.one;
            pRect.sizeDelta = Vector2.zero;

            Image pbg = pausePanel.AddComponent<Image>();
            pbg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

            Text pauseTitle = CreateText(pausePanel.transform, "PauseTitle", "TẠM DỪNG (PAUSED)", 54, new Vector2(0, 100), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Color.yellow, defaultFont);
            pauseTitle.alignment = TextAnchor.MiddleCenter;

            resumeButton = CreateButton(pausePanel.transform, "ResumeButton", "TIẾP TỤC (RESUME)", new Vector2(0, 0), new Vector2(280, 55), new Color(0.2f, 0.7f, 0.3f), defaultFont);
            pauseRestartButton = CreateButton(pausePanel.transform, "PauseRestartButton", "CHƠI LẠI (RESTART)", new Vector2(0, -75), new Vector2(280, 55), new Color(0.8f, 0.3f, 0.2f), defaultFont);

            pausePanel.SetActive(false);
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax, Color color, Font font)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(800, 100);

            Text txt = obj.AddComponent<Text>();
            txt.text = content;
            txt.font = font;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            return txt;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color btnColor, Font font)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            Image img = obj.AddComponent<Image>();
            img.color = btnColor;

            Button btn = obj.AddComponent<Button>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Text txt = textObj.AddComponent<Text>();
            txt.text = label;
            txt.font = font;
            txt.fontSize = 24;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            return btn;
        }
    }
}
