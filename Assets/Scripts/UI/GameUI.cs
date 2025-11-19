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
    
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    [Header("Screen Effects")]
    [SerializeField] private Image screenBorderEffect;
    [SerializeField] private float borderPulseSpeed = 2f;
    
    private SuperModeManager superModeManager;
    
    void Start()
    {
        superModeManager = FindObjectOfType<SuperModeManager>();
        
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
    }
    
    void Update()
    {
        UpdateScoreUI();
        UpdateTimerUI();
        UpdateGaugeUI();
        UpdateSuperModeUI();
        UpdatePauseMenu();
        UpdateScreenEffects();
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
        if (superModeManager == null) return;
        
        if (player1GaugeBar != null)
        {
            float gauge1 = superModeManager.GetPlayer1Gauge();
            player1GaugeBar.fillAmount = gauge1 / 100f;
            player1GaugeBar.color = gauge1 >= 100f ? gaugeFullColor : gaugeNormalColor;
        }
        
        if (player2GaugeBar != null)
        {
            float gauge2 = superModeManager.GetPlayer2Gauge();
            player2GaugeBar.fillAmount = gauge2 / 100f;
            player2GaugeBar.color = gauge2 >= 100f ? gaugeFullColor : gaugeNormalColor;
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
        
        bool shouldPulse = gauge1 >= 90f || gauge2 >= 90f;
        
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
    
    void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitToTitle();
        }
    }
}

