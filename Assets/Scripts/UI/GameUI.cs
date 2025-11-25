using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI suddenDeathText;

    [Header("Gauge UI")]
    [SerializeField] private Image player1GaugeBar;
    [SerializeField] private Image player2GaugeBar;
    [SerializeField] private Color gaugeNormalColor = Color.cyan;
    [SerializeField] private Color gaugeFullColor = Color.yellow;

    [Header("Super Mode UI")]
    [SerializeField] private GameObject player1SuperIcon;
    [SerializeField] private GameObject player2SuperIcon;
    [SerializeField] private TextMeshProUGUI player1SuperText;
    [SerializeField] private TextMeshProUGUI player2SuperText;

    [Header("Result UI")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI goalText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Screen Effects")]
    [SerializeField] private Image screenBorderEffect;
    [SerializeField] private float borderPulseSpeed = 2f;

    [Header("Power Up UI")]
    [SerializeField] private TextMeshProUGUI player1PowerUpText;
    [SerializeField] private TextMeshProUGUI player2PowerUpText;

    private SuperModeManager superModeManager;
    public static GameUI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        superModeManager = FindObjectOfType<SuperModeManager>();
        Debug.Log("[GameUI] Start - superModeManager=" + (superModeManager != null) +
                  ", player1GaugeBar=" + (player1GaugeBar != null) +
                  ", player2GaugeBar=" + (player2GaugeBar != null));

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (suddenDeathText != null)
            suddenDeathText.gameObject.SetActive(false);
        if (screenBorderEffect != null)
            screenBorderEffect.gameObject.SetActive(false);
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateScoreUI();
        UpdateTimerUI();
        UpdateGaugeUI();
        UpdateSuperModeUI();
        UpdatePauseMenu();
        UpdateScreenEffects();
        UpdateResultUI();
    }

    void UpdateScoreUI()
    {
        if (GameManager.Instance == null) return;

        if (player1ScoreText != null)
            player1ScoreText.text = GameManager.Instance.GetPlayer1Score().ToString();

        if (player2ScoreText != null)
            player2ScoreText.text = GameManager.Instance.GetPlayer2Score().ToString();
    }

    void UpdateTimerUI()
    {
        if (GameManager.Instance == null) return;

        float remainingTime = GameManager.Instance.GetRemainingTime();

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (suddenDeathText != null)
        {
            suddenDeathText.gameObject.SetActive(GameManager.Instance.IsSuddenDeath());
        }
    }

    void UpdateGaugeUI()
    {
        if (superModeManager == null)
        {
            Debug.LogWarning("[GameUI] SuperModeManager is null - gauge UI will not update");
            return;
        }

        if (player1GaugeBar != null)
        {
            float gauge1 = superModeManager.GetPlayer1Gauge();
            float maxGauge = superModeManager.GetMaxGauge();
            player1GaugeBar.fillAmount = maxGauge > 0f ? gauge1 / maxGauge : 0f;
            player1GaugeBar.color = gauge1 >= maxGauge ? gaugeFullColor : gaugeNormalColor;
            Debug.Log("[GameUI] P1 gauge=" + gauge1 + ", fill=" + player1GaugeBar.fillAmount);
        }
        else
        {
            Debug.LogWarning("[GameUI] player1GaugeBar is null");
        }

        if (player2GaugeBar != null)
        {
            float gauge2 = superModeManager.GetPlayer2Gauge();
            float maxGauge = superModeManager.GetMaxGauge();
            player2GaugeBar.fillAmount = maxGauge > 0f ? gauge2 / maxGauge : 0f;
            player2GaugeBar.color = gauge2 >= maxGauge ? gaugeFullColor : gaugeNormalColor;
            Debug.Log("[GameUI] P2 gauge=" + gauge2 + ", fill=" + player2GaugeBar.fillAmount);
        }
        else
        {
            Debug.LogWarning("[GameUI] player2GaugeBar is null");
        }
    }

    void UpdateSuperModeUI()
    {
        if (superModeManager == null) return;

        if (player1SuperIcon != null)
        {
            player1SuperIcon.SetActive(superModeManager.IsPlayer1InSuperMode());
        }

        if (player1SuperText != null)
        {
            player1SuperText.gameObject.SetActive(superModeManager.IsPlayer1InSuperMode());
            if (superModeManager.IsPlayer1InSuperMode())
            {
                player1SuperText.text = "슈퍼!";
            }
        }

        if (player2SuperIcon != null)
        {
            player2SuperIcon.SetActive(superModeManager.IsPlayer2InSuperMode());
        }

        if (player2SuperText != null)
        {
            player2SuperText.gameObject.SetActive(superModeManager.IsPlayer2InSuperMode());
            if (superModeManager.IsPlayer2InSuperMode())
            {
                player2SuperText.text = "슈퍼!";
            }
        }
    }

    void UpdatePauseMenu()
    {
        if (GameManager.Instance == null) return;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(GameManager.Instance.IsPaused());
        }
    }

    void UpdateScreenEffects()
    {
        if (superModeManager == null || screenBorderEffect == null) return;

        float gauge1 = superModeManager.GetPlayer1Gauge();
        float gauge2 = superModeManager.GetPlayer2Gauge();
        float maxGauge = superModeManager.GetMaxGauge();

        bool shouldPulse = (maxGauge > 0f) && (gauge1 >= maxGauge * 0.9f || gauge2 >= maxGauge * 0.9f);

        if (shouldPulse)
        {
            screenBorderEffect.gameObject.SetActive(true);
            float alpha = Mathf.PingPong(Time.time * borderPulseSpeed, 0.5f);
            Color color = screenBorderEffect.color;
            color.a = alpha;
            screenBorderEffect.color = color;
        }
        else
        {
            screenBorderEffect.gameObject.SetActive(false);
        }
    }

    void UpdateResultUI()
    {
        if (GameManager.Instance == null || resultText == null) return;

        if (!GameManager.Instance.IsGameActive())
        {
            int p1 = GameManager.Instance.GetPlayer1Score();
            int p2 = GameManager.Instance.GetPlayer2Score();

            string message;
            if (p1 > p2)
                message = "BLUE TEAM WINS!";
            else if (p2 > p1)
                message = "RED TEAM WINS!";
            else
                message = "DRAW!";

            resultText.text = message;
            resultText.gameObject.SetActive(true);
        }
        else
        {
            resultText.gameObject.SetActive(false);
        }
    }


    void OnResumeClicked()
    {
        if (GameManager.Instance != null)
        {
            Time.timeScale = 1f;
        }
    }

    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void ShowPowerUpEffect(int playerNumber, string effectName, float duration)
    {
        if (playerNumber == 1 && player1PowerUpText != null)
        {
            StartCoroutine(ShowPowerUpCoroutine(player1PowerUpText, effectName, duration));
        }
        else if (playerNumber == 2 && player2PowerUpText != null)
        {
            StartCoroutine(ShowPowerUpCoroutine(player2PowerUpText, effectName, duration));
        }
    }

    public void ShowGoalText()
    {
        if (goalText == null)
        {
            Debug.LogWarning("[GameUI] goalText is null, attempting to find it...");
            GameObject goalObj = GameObject.Find("GoalText"); // Assuming the object is named "GoalText"
            if (goalObj != null)
            {
                goalText = goalObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("[GameUI] GoalText object not found. Creating one dynamically...");
                CreateGoalText();
            }
        }

        if (goalText != null)
        {
            Debug.Log("[GameUI] ShowGoalText called - Starting Coroutine");
            StartCoroutine(ShowGoalTextCoroutine());
        }
        else
        {
            Debug.LogError("[GameUI] Failed to create GoalText!");
        }
    }

    void CreateGoalText()
    {
        GameObject goalObj = new GameObject("GoalText");
        goalObj.transform.SetParent(this.transform, false);
        
        goalText = goalObj.AddComponent<TextMeshProUGUI>();
        goalText.text = "GOAL!!!!!";
        goalText.fontSize = 120;
        goalText.alignment = TextAlignmentOptions.Center;
        goalText.color = Color.yellow;
        goalText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        
        // Add outline
        goalText.outlineColor = Color.black;
        goalText.outlineWidth = 0.5f;
        
        RectTransform rt = goalText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(800, 300);
        rt.anchoredPosition = Vector2.zero;
        
        goalObj.SetActive(false);
        Debug.Log("[GameUI] Created GoalText dynamically.");
    }

    System.Collections.IEnumerator ShowGoalTextCoroutine()
    {
        goalText.text = "GOAL!!!!!";
        goalText.gameObject.SetActive(true);
        goalText.transform.localScale = Vector3.zero;
        
        // Reset rotation
        goalText.transform.localRotation = Quaternion.identity;

        float duration = 0.6f;
        float elapsed = 0f;

        // Pop in with rotation
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Elastic ease out for scale
            float scale = Mathf.Sin(-13 * (progress + 1) * Mathf.PI / 2) * Mathf.Pow(2, -10 * progress) + 1;
            goalText.transform.localScale = Vector3.one * scale;
            
            // Spin effect
            float rotation = Mathf.Lerp(360f, 0f, 1f - Mathf.Pow(1f - progress, 3f)); // Cubic ease out
            goalText.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            
            yield return null;
        }
        
        goalText.transform.localScale = Vector3.one;
        goalText.transform.localRotation = Quaternion.identity;

        // Wait
        yield return new WaitForSeconds(1.5f);

        // Fade out
        elapsed = 0f;
        float fadeDuration = 0.5f;
        Color originalColor = goalText.color;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            goalText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        goalText.gameObject.SetActive(false);
        goalText.color = originalColor; // Reset color for next time
    }

    System.Collections.IEnumerator ShowPowerUpCoroutine(TextMeshProUGUI textUI, string effectName, float duration)
    {
        textUI.text = effectName;
        textUI.gameObject.SetActive(true);

        float elapsed = 0f;
        float popDuration = 0.3f;
        Vector3 originalScale = textUI.transform.localScale;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / popDuration;
            float scale = Mathf.Lerp(0f, 1.2f, progress);
            textUI.transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / 0.2f;
            float scale = Mathf.Lerp(1.2f, 1f, progress);
            textUI.transform.localScale = originalScale * scale;
            yield return null;
        }

        textUI.transform.localScale = originalScale;

        float remainingTime = duration - popDuration - 0.2f;
        if (remainingTime > 1f)
        {
            yield return new WaitForSeconds(remainingTime - 1f);

            elapsed = 0f;
            Color originalColor = textUI.color;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed);
                textUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            textUI.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(remainingTime);
        }

        textUI.gameObject.SetActive(false);
    }

    void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitToTitle();
        }
    }
}

