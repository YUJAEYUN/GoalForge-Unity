using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerNumber = 1;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private float lastDashTime;
    private float horizontalInput;

    [HideInInspector] public bool isSuperMode = false;
    [HideInInspector] public float superModeSpeedMultiplier = 1f;
    [HideInInspector] public float superModeJumpMultiplier = 1f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearDamping = 2f;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 1;
        }
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
        if (playerNumber == 1)
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

            if (Input.GetKeyDown(KeyCode.RightShift) && Time.time >= lastDashTime + dashCooldown)
            {
                Dash();
            }
        }
        else if (playerNumber == 2)
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
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

        if (playerNumber == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f;
            if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f;
        }
        else if (playerNumber == 2)
        {
            if (Input.GetKey(KeyCode.W)) moveY = 1f;
            if (Input.GetKey(KeyCode.S)) moveY = -1f;
        }
        moveY *= currentSpeed;
        rb.linearVelocity = new Vector2(moveX, moveY);
    }

    void Jump()
    {
    }

    void Dash()
    {
        float dashDirection = horizontalInput != 0 ? horizontalInput : (transform.localScale.x > 0 ? 1 : -1);
        rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
        lastDashTime = Time.time;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDashSound();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball != null)
            {
                Vector2 kickDirection = (collision.transform.position - transform.position).normalized;
                float kickPower = 6f * (isSuperMode ? 1.5f : 1f);
                ball.Kick(kickDirection * kickPower);

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayKickSound();
                }
            }
        }
    }
}

