using UnityEngine;

public class PlayerScript : BaseCharacter
{
    [SerializeField] private float jumpForce = 10f;

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
            rb.linearVelocity = new Vector2(xInput * speed, rb.linearVelocity.y);
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