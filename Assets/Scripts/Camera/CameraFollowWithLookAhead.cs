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

    [Header("SCREEN SHAKE! 🔥")]
    [SerializeField] private float shakeMagnitude = 0.3f;
    [SerializeField] private float shakeDuration = 0.2f;

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
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TriggerShake(0.5f, 1.0f);
    }*/

    void LateUpdate()
    {
        UpdateShake();

        float targetLookAheadX = 0f;
        if (Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            targetLookAheadX = Mathf.Sign(playerRb.linearVelocity.x) * lookAheadDistance;
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref lookAheadVelocity, lookAheadSmoothTime);

        Vector3 targetPosition = new Vector3(
            player.position.x + currentLookAheadX + baseOffset.x,
            player.position.y + baseOffset.y,
            transform.position.z
        );

        // First: normal smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Then: ADD shake directly (no smoothing applied to shake!)
        transform.position += shakeOffset;
    }

    private void UpdateShake()
    {
        if (currentShakeTime > 0)
        {
            // Random shake offset
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude,
                0
            );
            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero; // Reset shake
        }
    }

    // PUBLIC METHOD - Call this from Health.cs!
    public void TriggerShake(float duration , float magnitude )
    {
        currentShakeTime = duration;
        shakeMagnitude = magnitude;
    }
}