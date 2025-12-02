using System.Collections;
using UnityEngine;

public class CharacterType : MonoBehaviour
{
    public enum Type
    {
        Son,
        Ronaldo,
        Messi,
        Haaland
    }

    [Header("Character Settings")]
    [SerializeField] private Type characterType = Type.Son;

    [Header("Visual Effects")]
    [SerializeField] private GameObject superModeVFX;
    [SerializeField] private Color superModeColor = Color.yellow;

    [Header("Super Mode Common Settings")]
    [SerializeField] private float superDuration = 3f;

    [Header("Son Super Settings")]
    [SerializeField] private float sonSpeedMultiplier = 2.0f;

    [Header("Ronaldo Super Settings")]
    [SerializeField] private float ronaldoStunDuration = 2f;

    [Header("Haaland Super Shot Settings")]
    [SerializeField] private float haalandShotPowerMultiplier = 10.0f;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    private Vector3 originalScale;
    private Color originalColor;

    private bool isSuperModeActive = false;
    private Coroutine superRoutine;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void ActivateSuperMode()
    {
        if (isSuperModeActive) return;
        isSuperModeActive = true;

        // 캐릭터별 발동 사운드
        if (SoundManager.Instance != null)
        {
            switch (characterType)
            {
                case Type.Son:
                    SoundManager.Instance.PlaySonSuperSound();
                    break;
                case Type.Ronaldo:
                    SoundManager.Instance.PlayRonaldoSuperSound();
                    break;
                case Type.Messi:
                    SoundManager.Instance.PlayMessiSuperSound();
                    break;
                case Type.Haaland:
                    SoundManager.Instance.PlayHaalandSuperSound();
                    break;
            }
        }

        // 캐릭터별 능력 적용
        switch (characterType)
        {
            case Type.Son:
                ActivateSonSuper();
                break;
            case Type.Ronaldo:
                ActivateRonaldoSuper();
                break;
            case Type.Messi:
                ActivateMessiSuper();
                break;
            case Type.Haaland:
                ActivateHaalandSuper();
                break;
        }

        ApplyVisualEffects();

        if (superDuration > 0f)
        {
            if (superRoutine != null) StopCoroutine(superRoutine);
            superRoutine = StartCoroutine(SuperModeTimer());
        }
    }

    public void DeactivateSuperMode()
    {
        if (!isSuperModeActive) return;
        isSuperModeActive = false;

        transform.localScale = originalScale;

        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = 1f;
            playerController.superModeJumpMultiplier = 1f;
            playerController.isSuperMode = false;

            // 메시 슈퍼 드리블 종료
            playerController.keepBallInSuperMode = false;
            // 홀란드 파워샷 종료
            playerController.enableHaalandPowerShot = false;
        }

        RemoveVisualEffects();
    }

    IEnumerator SuperModeTimer()
    {
        yield return new WaitForSeconds(superDuration);
        DeactivateSuperMode();
    }

    // ───────── 손흥민: 스피드 업 ─────────
    void ActivateSonSuper()
    {
        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = sonSpeedMultiplier;
            playerController.superModeJumpMultiplier = 1f;
            playerController.isSuperMode = true;
        }
    }

    // ───────── 호날두: 상대 스턴 ─────────
    void ActivateRonaldoSuper()
    {
        if (playerController != null)
        {
            playerController.isSuperMode = true;
        }

        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach (var p in players)
        {
            if (p == playerController) continue;

            StartCoroutine(StunTargetPlayer(p, ronaldoStunDuration));
        }
    }

    IEnumerator StunTargetPlayer(PlayerController target, float duration)
    {
        if (target == null) yield break;

        target.isStunned = true;

        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            RigidbodyConstraints2D originalConstraints = targetRb.constraints;

            targetRb.linearVelocity = Vector2.zero;
            targetRb.angularVelocity = 0f;
            targetRb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

            yield return new WaitForSeconds(duration);

            targetRb.constraints = originalConstraints;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        target.isStunned = false;
    }

    // ───────── 메시: 일정 시간 내에 공을 맞추면 발 앞에 붙는 슈퍼 드리블 ─────────
    void ActivateMessiSuper()
    {
        if (playerController != null)
        {
            playerController.superModeSpeedMultiplier = 0.8f;
            playerController.superModeJumpMultiplier = 1f;
            playerController.isSuperMode = true;

            // 이 상태에서 공과 부딪히면 PlayerController에서 Attach 처리
            playerController.keepBallInSuperMode = true;
        }

        Debug.Log("Messi Super Activated: waiting to touch the ball.");
    }

    // ───────── 홀란드: 슈퍼 지속 동안 모든 슛 강화 ─────────
    void ActivateHaalandSuper()
    {
        if (playerController != null)
        {
            playerController.isSuperMode = true;
            playerController.enableHaalandPowerShot = true;
            playerController.haalandShotPowerMultiplier = haalandShotPowerMultiplier;
        }

        Debug.Log("Haaland Super Ready: next shot is PowerShot.");
    }

    void ApplyVisualEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = superModeColor;
        }

        if (superModeVFX != null)
        {
            superModeVFX.SetActive(true);
        }
    }

    void RemoveVisualEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (superModeVFX != null)
        {
            superModeVFX.SetActive(false);
        }
    }

    public Type GetCharacterType() => characterType;
    public bool IsSuperModeActive() => isSuperModeActive;
}
