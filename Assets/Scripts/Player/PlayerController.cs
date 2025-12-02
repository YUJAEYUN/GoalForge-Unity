using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerNumber = 1;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Visual / Animation")]
    [SerializeField] private Transform visualRoot;

    private Rigidbody2D rb;
    private float lastDashTime;
    private float horizontalInput;

    private Animator anim;

    [HideInInspector] public bool isSuperMode = false;
    [HideInInspector] public float superModeSpeedMultiplier = 1f;
    [HideInInspector] public float superModeJumpMultiplier = 1f;

    [HideInInspector] public bool hasShield = false;
    [HideInInspector] public bool hasMegaBall = false;
    [HideInInspector] public float megaBallMultiplier = 1f;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public bool hasReversedControls = false;

    // 메시: 공 안 떨어지는 슈퍼 드리블
    [HideInInspector] public bool keepBallInSuperMode = false;

    // 홀란드: 슈퍼 동안 모든 슛이 강력해짐
    [HideInInspector] public bool enableHaalandPowerShot = false;
    [HideInInspector] public float haalandShotPowerMultiplier = 2.5f;

    public int GetPlayerNumber() => playerNumber;
    public void SetPlayerNumber(int number) => playerNumber = number;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearDamping = 2f;

        if (visualRoot == null)
            visualRoot = transform.Find("Visual");

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null && visualRoot != null)
            sr = visualRoot.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.sortingOrder = 1;

        if (visualRoot != null)
            anim = visualRoot.GetComponent<Animator>();
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        if (isStunned)
        {
            horizontalInput = 0f;
            return;
        }

        float inputMultiplier = hasReversedControls ? -1f : 1f;

        if (playerNumber == 1)
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f * inputMultiplier;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f * inputMultiplier;

            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
            {
                Dash();
            }
        }
        else if (playerNumber == 2)
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f * inputMultiplier;
            if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f * inputMultiplier;

            if (Input.GetKeyDown(KeyCode.RightShift) && Time.time >= lastDashTime + dashCooldown)
            {
                Dash();
            }
        }
    }

    void Move()
    {
        float currentSpeed = moveSpeed * superModeSpeedMultiplier;
        float moveX = horizontalInput * currentSpeed;
        float moveY = 0f;

        float inputMultiplier = hasReversedControls ? -1f : 1f;

        if (playerNumber == 1)
        {
            if (Input.GetKey(KeyCode.W)) moveY = 1f * inputMultiplier;
            if (Input.GetKey(KeyCode.S)) moveY = -1f * inputMultiplier;
        }
        else if (playerNumber == 2)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f * inputMultiplier;
            if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f * inputMultiplier;
        }

        moveY *= currentSpeed;

        rb.linearVelocity = new Vector2(moveX, moveY);

        if (visualRoot != null)
        {
            if (moveX > 0.01f)
            {
                visualRoot.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (moveX < -0.01f)
            {
                visualRoot.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        if (anim != null)
        {
            bool isMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;
            anim.SetFloat("Speed", isMoving ? 1f : 0f);
        }
    }

    void Jump()
    {
    }

    void Dash()
    {
        float dashDirection = horizontalInput != 0 ? horizontalInput :
            (visualRoot != null && visualRoot.localScale.x > 0 ? 1 : -1);

        rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
        lastDashTime = Time.time;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDashSound();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ───────── 공과 충돌 (슛 / 드리블 / 필살슛 처리) ─────────
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball != null)
            {
                Vector2 kickDirection = (collision.transform.position - transform.position).normalized;
                float kickPower = 3.5f * (isSuperMode ? 1.2f : 1f) * megaBallMultiplier;

                SuperModeManager superModeManager = FindObjectOfType<SuperModeManager>();

                // 메시 슈퍼 드리블
                if (keepBallInSuperMode)
                {
                    ball.AttachTo(this);

                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayKickSound();

                    if (superModeManager != null)
                        superModeManager.OnBallTouch(this);

                    return;
                }

                // ⭐ 홀란드 파워슛: 단 한 번만 강한 슛 가능 (예전 버전)
                if (enableHaalandPowerShot)
                {
                    kickPower *= haalandShotPowerMultiplier;
                    ball.TriggerPowerShotEffect();  // 빨간 이펙트

                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayHaalandPowerShotKickSound();

                    enableHaalandPowerShot = false;  // ⬅ 한 번 쓰면 바로 꺼짐
                }

                ball.Kick(kickDirection * kickPower);

                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayKickSound();

                if (superModeManager != null)
                    superModeManager.OnBallTouch(this);
            }
        }
    }
}
