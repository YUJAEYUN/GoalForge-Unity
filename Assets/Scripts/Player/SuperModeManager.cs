using UnityEngine;
using System.Collections;

public class SuperModeManager : MonoBehaviour
{
    [Header("Gauge Settings")]
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float autoChargeRate = 1f;
    [SerializeField, Range(0f, 1f)] private float ballTouchBonusPercent = 0.1f; // 공에 닿을 때 최대 게이지의 비율 (0.1 = 10%)

    private const float AutoChargeSpeedMultiplier = 5f;

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
        {
            //Debug.Log("[SuperModeManager] Game not active - skipping gauge update");
            return;
        }

        AutoChargeGauge();

        CheckAndActivateSuperMode();
    }

    void AutoChargeGauge()
    {
        float rate = autoChargeRate * AutoChargeSpeedMultiplier;

        if (!player1InSuperMode && player1Gauge < maxGauge)
        {
            player1Gauge += rate * Time.deltaTime;
            player1Gauge = Mathf.Min(player1Gauge, maxGauge);
        }

        if (!player2InSuperMode && player2Gauge < maxGauge)
        {
            player2Gauge += rate * Time.deltaTime;
            player2Gauge = Mathf.Min(player2Gauge, maxGauge);
        }
    }

    void CheckAndActivateSuperMode()
    {
        if (player1Gauge >= maxGauge && !player1InSuperMode)
        {
            ActivateSuperMode(1);
        }

        if (player2Gauge >= maxGauge && !player2InSuperMode)
        {
            ActivateSuperMode(2);
        }
    }

    public void OnBallTouch(PlayerController player)
    {
        if (player == null)
        {
            Debug.LogWarning("[SuperModeManager] OnBallTouch called with null player");
            return;
        }

        int playerNumber = player.GetPlayerNumber();

        if (playerNumber == 1 && !player1InSuperMode)
        {
            if (player1 == null)
            {
                player1 = player;
                Debug.Log("[SuperModeManager] Bound player1 reference to " + player.name);
            }

            float bonus = maxGauge * ballTouchBonusPercent;
            player1Gauge += bonus;
            player1Gauge = Mathf.Min(player1Gauge, maxGauge);
            Debug.Log("[SuperModeManager] Ball touched by Player1, gauge=" + player1Gauge);
        }
        else if (playerNumber == 2 && !player2InSuperMode)
        {
            if (player2 == null)
            {
                player2 = player;
                Debug.Log("[SuperModeManager] Bound player2 reference to " + player.name);
            }

            float bonus = maxGauge * ballTouchBonusPercent;
            player2Gauge += bonus;
            player2Gauge = Mathf.Min(player2Gauge, maxGauge);
            Debug.Log("[SuperModeManager] Ball touched by Player2, gauge=" + player2Gauge);
        }
        else
        {
            Debug.Log("[SuperModeManager] OnBallTouch by player not matched to player1/player2 or currently in super mode. " +
                      "player=" + player.name + ", playerNumber=" + playerNumber +
                      ", player1=" + (player1 != null ? player1.name : "null") +
                      ", player2=" + (player2 != null ? player2.name : "null"));
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
    public float GetMaxGauge() => maxGauge;
    public bool IsPlayer1InSuperMode() => player1InSuperMode;
    public bool IsPlayer2InSuperMode() => player2InSuperMode;
}

