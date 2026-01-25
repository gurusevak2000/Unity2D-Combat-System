using UnityEngine;

public class PlayerScript : BaseCharacter
{
    [SerializeField] private float jumpForce = 10f;
    private const float MIN_PLAYER_X = -8f;  // Adjust this value as needed

    private float xInput;  // Keeps track of horizontal input

    protected override void Update()
    {
        // ALWAYS read input every frame (even if knocked, input should still be read)
        HandleInput();

        // Base Update handles: ground check, flip, animation params, and calls HandleMovement()
        // But only allow movement if not knocked
        if (!isKnocked)
        {
            base.Update();  // This will call HandleMovement() with current xInput
        }
        else
        {
            // Still update ground and animation even when knocked
            HandleGroundCheckCollions();
            HandleAnimation();
            HandleFlip();
        }
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && canMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    protected override void HandleMovement()
    {
        if (canMove)
        {
            const float MIN_PLAYER_X = -25f;  // Slightly INSIDE camera left edge
            
            float targetVelocityX = xInput * speed;
            
            // BLOCK moving left off screen
            if (transform.position.x <= MIN_PLAYER_X && xInput < 0)
            {
                targetVelocityX = 0;
            }
            
            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        }
    }

    public virtual void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    //Draw Gizmo method 
    protected override void OnDrawGizmosSelected()
    {
        // Your existing ground/facing gizmos...

        if (TryGetComponent<PlayerCombat>(out var combat))
        {
            combat.DrawAttackGizmos();  // Clean, encapsulated, centralized
        }
    }
    
}