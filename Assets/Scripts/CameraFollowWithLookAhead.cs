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

    private Vector3 velocity;
    private float currentLookAheadX;
    private float lookAheadVelocity;

    void LateUpdate()
    {
        float targetLookAheadX = 0f;

        // Detect horizontal movement
        if (Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            targetLookAheadX = Mathf.Sign(playerRb.linearVelocity.x) * lookAheadDistance;
        }

        // Smooth look-ahead transition
        currentLookAheadX = Mathf.SmoothDamp(
            currentLookAheadX,
            targetLookAheadX,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        Vector3 targetPosition = new Vector3(
            player.position.x + currentLookAheadX + baseOffset.x,
            player.position.y + baseOffset.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
