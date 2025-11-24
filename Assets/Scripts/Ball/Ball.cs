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

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private float lastGroundTouchTime;
    private bool isTouchingGround;
    
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 2;
        }

        // Add TrailRenderer for polish
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
        CheckGroundContact();
        ApplyMagnetEffect();
        LimitVelocity();
    }
    
    void CheckGroundContact()
    {
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

            SuperModeManager superModeManager = Object.FindAnyObjectByType<SuperModeManager>();
            if (superModeManager != null)
            {
                Debug.Log("[Ball] Calling SuperModeManager.OnBallTouch for " + player.name);
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
    }

    public void Kick(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPosition, 0.5f);
    }
}

