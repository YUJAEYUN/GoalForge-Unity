using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Physics")]
    [SerializeField] private float mass = 0.4f;
    [SerializeField] private float bounciness = 0.6f;
    [SerializeField] private float maxVelocity = 15f;

    [Header("Magnet Effect")]
    [SerializeField] private float magnetForce = 0.5f;
    [SerializeField] private float magnetActivationTime = 3f;
    [SerializeField] private Vector2 centerPosition = Vector2.zero;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Dribble Attach (Messi Super)")]
    [SerializeField] private float followOffsetX = 0.5f;   // 플레이어 기준 X 오프셋
    [SerializeField] private float followOffsetY = -0.1f;  // 플레이어 기준 Y 오프셋

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color powerShotColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private float powerShotColorDuration = 0.3f;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private float lastGroundTouchTime;
    private bool isTouchingGround;

    // 메시 슈퍼 드리블용: 공을 붙잡고 있는 플레이어
    private PlayerController holder;

    private SpriteRenderer spriteRenderer;
    private Coroutine powerShotRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        rb.mass = mass;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearDamping = 1f;

        if (circleCollider != null)
        {
            circleCollider.radius = 0.25f;
        }

        PhysicsMaterial2D ballMaterial = new PhysicsMaterial2D("BallMaterial");
        ballMaterial.bounciness = bounciness;
        ballMaterial.friction = 0.3f;
        circleCollider.sharedMaterial = ballMaterial;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 2;
            normalColor = spriteRenderer.color;
        }

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.3f;
            trail.startWidth = 0.3f;
            trail.endWidth = 0f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = new Color(1f, 1f, 1f, 0.5f);
            trail.endColor = new Color(1f, 1f, 1f, 0f);
            trail.sortingOrder = 1;
        }
    }

    void Start()
    {
        lastGroundTouchTime = Time.time;
    }

    void FixedUpdate()
    {
        // 메시 슈퍼 드리블: holder가 있고, keepBallInSuperMode면 플레이어를 따라다님
        if (holder != null && holder.keepBallInSuperMode)
        {
            FollowHolder();
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }
        else if (holder != null && !holder.keepBallInSuperMode)
        {
            // 슈퍼 모드 종료 시 자동 해제
            Detach();
        }

        CheckGroundContact();
        ApplyMagnetEffect();
        LimitVelocity();
    }

    void CheckGroundContact()
    {
        // 필요하면 Raycast로 구현 가능. 지금은 lastGroundTouchTime만 사용
    }

    void ApplyMagnetEffect()
    {
        float timeSinceLastTouch = Time.time - lastGroundTouchTime;

        if (timeSinceLastTouch >= magnetActivationTime)
        {
            Vector2 directionToCenter = (centerPosition - (Vector2)transform.position).normalized;
            rb.AddForce(directionToCenter * magnetForce, ForceMode2D.Force);
        }
    }

    void LimitVelocity()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            Debug.Log("[Ball] Collision with player " + player.name + " (tag=" + collision.gameObject.tag + ")");
            lastGroundTouchTime = Time.time;

            SuperModeManager superModeManager = FindObjectOfType<SuperModeManager>();
            if (superModeManager != null)
            {
                superModeManager.OnBallTouch(player);
            }
            else
            {
                Debug.LogWarning("[Ball] SuperModeManager not found in scene");
            }
        }
        else
        {
            Debug.Log("[Ball] Collision with non-player " + collision.gameObject.name + " (tag=" + collision.gameObject.tag + ")");
        }
    }

    public void ResetPosition(Vector2 position)
    {
        transform.position = position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        lastGroundTouchTime = Time.time;
        Detach();
    }

    public void Kick(Vector2 force)
    {
        Detach();
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    // ───────── 메시 슈퍼 드리블용 Attach / Detach ─────────
    public void AttachTo(PlayerController player)
    {
        holder = player;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        lastGroundTouchTime = Time.time;
    }

    public void Detach()
    {
        holder = null;
    }

    public bool IsAttachedTo(PlayerController player)
    {
        return holder == player;
    }

    void FollowHolder()
    {
        if (holder == null) return;

        Vector3 basePos = holder.transform.position;

        float facingDir = 1f;
        Transform visual = holder.transform.Find("Visual");
        if (visual != null)
        {
            facingDir = Mathf.Sign(visual.localScale.x);
        }

        Vector3 targetPos = new Vector3(
            basePos.x + followOffsetX * facingDir,
            basePos.y + followOffsetY,
            transform.position.z
        );

        transform.position = targetPos;
    }

    // ───────── 홀란드 파워샷: 공 색 변화 ─────────
    public void TriggerPowerShotEffect()
    {
        if (spriteRenderer == null) return;

        if (powerShotRoutine != null)
        {
            StopCoroutine(powerShotRoutine);
        }
        powerShotRoutine = StartCoroutine(PowerShotFlash());
    }

    private IEnumerator PowerShotFlash()
    {
        spriteRenderer.color = powerShotColor;
        yield return new WaitForSeconds(powerShotColorDuration);
        spriteRenderer.color = normalColor;
        powerShotRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPosition, 0.5f);
    }
}
