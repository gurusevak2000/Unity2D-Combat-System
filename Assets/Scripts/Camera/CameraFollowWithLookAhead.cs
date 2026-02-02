using UnityEngine;

public class CameraFollowWithLookAhead : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector2 baseOffset = new Vector2(0, 3f);

    [Header("Look Ahead Settings")]
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSmoothTime = 0.1f;

    [Header("CAMERA BOUNDS")]
    [SerializeField] private float minCameraX = -8f;  // ← LEFT WALL POSITION
    [SerializeField] private float maxCameraX = 300f; // ← DYNAMIC RIGHT (chunks)

    [Header("SCREEN SHAKE! ")]
    [SerializeField] private float shakeMagnitude;

    // Shake variables
    private Vector3 originalPos;
    private float currentShakeTime;
    private Vector3 shakeOffset;

    private Vector3 velocity;
    private float currentLookAheadX;
    private float lookAheadVelocity;

    void Start()
    {
        originalPos = transform.localPosition;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerRb == null) playerRb = player?.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        UpdateShake();

        // Look ahead calculation (unchanged)
        float targetLookAheadX = 0f;
        if (Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            targetLookAheadX = Mathf.Sign(playerRb.linearVelocity.x) * lookAheadDistance;
        }
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref lookAheadVelocity, lookAheadSmoothTime);

        // BASE TARGET (your original logic)
        Vector3 targetPosition = new Vector3(
            player.position.x + currentLookAheadX + baseOffset.x,
            player.position.y + baseOffset.y,
            transform.position.z
        );

        //CRITICAL: CLAMP CAMERA POSITION
        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        targetPosition.x = Mathf.Clamp(targetPosition.x, 
            minCameraX + halfWidth,      // Left edge + camera width
            maxCameraX - halfWidth       // Right edge - camera width
        );

        // Smooth follow WITHIN bounds
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime) + shakeOffset;
    }

    // ChunkManager calls this to extend right boundary
    public void UpdateMaxCameraX(float newMaxX)
    {
        maxCameraX = Mathf.Max(maxCameraX, newMaxX);
    }

    private void UpdateShake()
    {
        if (currentShakeTime > 0)
        {
            shakeOffset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude,
                UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude,
                0
            );
            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void TriggerShake(float duration, float? forcedMagnitude = null)
    {
        currentShakeTime = duration;
        
        if (forcedMagnitude.HasValue)
        {
            shakeMagnitude = forcedMagnitude.Value;
        }
        
    }

    // Visual bounds in Scene view
    private void OnDrawGizmosSelected()
    {
        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minCameraX, -100, 0), new Vector3(minCameraX, 100, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(maxCameraX, -100, 0), new Vector3(maxCameraX, 100, 0));
    }
}