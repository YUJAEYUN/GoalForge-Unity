using UnityEngine;
using System.Collections;

public class SuperModeManager : MonoBehaviour
{
    [Header("Gauge Settings")]
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float autoChargeRate = 1f; // 1점/초
    [SerializeField] private float ballTouchBonus = 10f;
    
    [Header("Super Mode Settings")]
    [SerializeField] private float superModeDuration = 3f;
    [SerializeField] private float superModeCooldown = 8f;
    
    [Header("Player References")]
    [SerializeField] private PlayerController player1;
    [SerializeField] private PlayerController player2;
    
    [Header("Audio")]
    [SerializeField] private AudioClip superModeActivateSound;
    
    private float player1Gauge = 0f;
    private float player2Gauge = 0f;
    
    private bool player1InSuperMode = false;
    private bool player2InSuperMode = false;
    private float player1LastSuperModeTime = -100f;
    private float player2LastSuperModeTime = -100f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        if (player1 == null)
        {
            GameObject p1 = GameObject.FindGameObjectWithTag("Player1");
            if (p1 != null) player1 = p1.GetComponent<PlayerController>();
        }
        
        if (player2 == null)
        {
            GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
            if (p2 != null) player2 = p2.GetComponent<PlayerController>();
        }
    }
    
    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive())
            return;
        
        AutoChargeGauge();
        
        CheckAndActivateSuperMode();
    }
    
    void AutoChargeGauge()
    {
        if (!player1InSuperMode && player1Gauge < maxGauge)
        {
            player1Gauge += autoChargeRate * Time.deltaTime;
            player1Gauge = Mathf.Min(player1Gauge, maxGauge);
        }
        
        if (!player2InSuperMode && player2Gauge < maxGauge)
        {
            player2Gauge += autoChargeRate * Time.deltaTime;
            player2Gauge = Mathf.Min(player2Gauge, maxGauge);
        }
    }
    
    void CheckAndActivateSuperMode()
    {
        if (player1Gauge >= maxGauge && !player1InSuperMode)
        {
            float timeSinceLastSuperMode = Time.time - player1LastSuperModeTime;
            if (timeSinceLastSuperMode >= superModeCooldown)
            {
                ActivateSuperMode(1);
            }
        }
        
        if (player2Gauge >= maxGauge && !player2InSuperMode)
        {
            float timeSinceLastSuperMode = Time.time - player2LastSuperModeTime;
            if (timeSinceLastSuperMode >= superModeCooldown)
            {
                ActivateSuperMode(2);
            }
        }
    }
    
    public void OnBallTouch(PlayerController player)
    {
        if (player == player1 && !player1InSuperMode)
        {
            player1Gauge += ballTouchBonus;
            player1Gauge = Mathf.Min(player1Gauge, maxGauge);
        }
        else if (player == player2 && !player2InSuperMode)
        {
            player2Gauge += ballTouchBonus;
            player2Gauge = Mathf.Min(player2Gauge, maxGauge);
        }
    }
    
    void ActivateSuperMode(int playerNumber)
    {
        if (playerNumber == 1 && player1 != null)
        {
            player1InSuperMode = true;
            player1Gauge = 0f;
            player1LastSuperModeTime = Time.time;
            
            CharacterType charType = player1.GetComponent<CharacterType>();
            if (charType != null)
            {
                charType.ActivateSuperMode();
            }
            
            StartCoroutine(DeactivateSuperModeAfterDuration(1));
            
            PlaySuperModeSound();
            Debug.Log("Player 1 Super Mode Activated!");
        }
        else if (playerNumber == 2 && player2 != null)
        {
            player2InSuperMode = true;
            player2Gauge = 0f;
            player2LastSuperModeTime = Time.time;
            
            CharacterType charType = player2.GetComponent<CharacterType>();
            if (charType != null)
            {
                charType.ActivateSuperMode();
            }
            
            StartCoroutine(DeactivateSuperModeAfterDuration(2));
            
            PlaySuperModeSound();
            Debug.Log("Player 2 Super Mode Activated!");
        }
    }
    
    IEnumerator DeactivateSuperModeAfterDuration(int playerNumber)
    {
        yield return new WaitForSeconds(superModeDuration);
        
        if (playerNumber == 1)
        {
            player1InSuperMode = false;
            if (player1 != null)
            {
                CharacterType charType = player1.GetComponent<CharacterType>();
                if (charType != null)
                {
                    charType.DeactivateSuperMode();
                }
            }
            Debug.Log("Player 1 Super Mode Deactivated!");
        }
        else if (playerNumber == 2)
        {
            player2InSuperMode = false;
            if (player2 != null)
            {
                CharacterType charType = player2.GetComponent<CharacterType>();
                if (charType != null)
                {
                    charType.DeactivateSuperMode();
                }
            }
            Debug.Log("Player 2 Super Mode Deactivated!");
        }
    }
    
    void PlaySuperModeSound()
    {
        if (superModeActivateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(superModeActivateSound);
        }
    }
    
    public float GetPlayer1Gauge() => player1Gauge;
    public float GetPlayer2Gauge() => player2Gauge;
    public bool IsPlayer1InSuperMode() => player1InSuperMode;
    public bool IsPlayer2InSuperMode() => player2InSuperMode;
}

