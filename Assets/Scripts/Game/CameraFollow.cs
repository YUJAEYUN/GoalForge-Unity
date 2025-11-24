using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // The object to follow (usually the Ball)
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public Vector2 minLimit = new Vector2(-5f, -3f); // Camera bounds
    public Vector2 maxLimit = new Vector2(5f, 3f);

    [Header("Zoom")]
    public float defaultZoom = 5f;
    public float zoomSpeed = 2f;
    private Camera cam;

    // Shake
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;
    private Vector3 initialPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            // Auto-find ball if not assigned
            Ball ball = Object.FindAnyObjectByType<Ball>();
            if (ball != null) target = ball.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Follow Target
        Vector3 desiredPosition = target.position + offset;
        
        // Clamp position to keep camera within stadium bounds
        float clampedX = Mathf.Clamp(desiredPosition.x, minLimit.x, maxLimit.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minLimit.y, maxLimit.y);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        // Smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 2. Screen Shake
        if (shakeDuration > 0)
        {
            transform.localPosition = transform.position + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    public void ZoomIn(float targetSize)
    {
        if (cam != null)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
    }
    
    public void ResetZoom()
    {
        if (cam != null)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultZoom, Time.deltaTime * zoomSpeed);
    }
}
