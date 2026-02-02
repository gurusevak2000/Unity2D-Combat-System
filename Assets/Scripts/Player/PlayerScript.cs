using System.Collections;
using UnityEngine;

public class PlayerScript : BaseCharacter
{
    [SerializeField] private float jumpForce = 10f;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;
    
    [Header("Dash Attack")]
    public bool canDashAttack = true;

    private const float MIN_PLAYER_X = -25f;
    private float xInput;

    // Dash state
    public bool isDashing { get; private set; } = false;
    private float lastDashTime;
    private bool isInvincible = false;
    private Coroutine dashCoroutine;
    private Coroutine dashAttackCoroutine;

    protected override void Update()
    {
        // ALWAYS read input — no matter what state we're in
        HandleInput();

        // TEMP DEBUG - press H to see state
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"canMove = {canMove} | isKnocked = {isKnocked} | " +
                    $"xVelocity param = {animator.GetFloat("xVelocity"):F2} | " +
                    $"Current state = {state.shortNameHash} ({state.IsName("Hit")}) | " +
                    $"Attack trigger = {animator.GetBool("Attack")}");
        }*/

        // Core logic: always run these unless fully locked
        if (!isKnocked)
        {
            base.Update();           // ← runs HandleMovement(), HandleAnimation(), HandleGroundCheckCollions(), HandleFlip()
        }
        else
        {
            // During knockback: still update visuals & ground
            HandleGroundCheckCollions();
            HandleAnimation();       // ← important: keeps IsGrounded correct
            HandleFlip();            // ← but we already block flip inside HandleFlip()
            
        }
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        // Jump (unchanged)
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && canMove && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // NEW D+SHIFT / A+SHIFT DASH
        if (canMove && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                animator.SetBool("IsDashing", true);
                StartDirectionalDash(1f);  // Right dash
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                animator.SetBool("IsDashing", true);
                StartDirectionalDash(-1f); // Left dash
            }
        }
    }

    private void StartDirectionalDash(float direction)
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        dashCoroutine = StartCoroutine(DirectionalDashCoroutine(direction));
    }

    public bool IsInvincible => isInvincible;
    private IEnumerator DirectionalDashCoroutine(float dashDir)
    {
        isDashing = true;
        isInvincible = true;
        animator.SetBool("IsDashing", true);
        lastDashTime = Time.time;

        // Precise directional dash
        Vector2 dashVelocity = new Vector2(dashDir * dashSpeed, 0);
        rb.linearVelocity = dashVelocity;

        yield return new WaitForSeconds(dashDuration);
        animator.SetBool("IsDashing", false);
        yield return new WaitForSeconds(dashCooldown - dashDuration);

        isDashing = false;
        isInvincible = false;
        animator.SetBool("IsDashing", false);
    }
    
    // Called from PlayerCombat for W+Shift dash attack
    public void TriggerDashAttack()
    {
        if (!canDashAttack || dashAttackCoroutine != null) return;

        dashAttackCoroutine = StartCoroutine(DashAttackCoroutine());
    }

    private IEnumerator DashAttackCoroutine()
    {
        isDashing = true;
        isInvincible = true;
        animator.SetTrigger("DashAttack");
        canDashAttack = false;

        // POWERFUL dash attack forward
        Vector2 dashVelocity = new Vector2(facingDirection * dashSpeed * 1.3f, 0);
        rb.linearVelocity = dashVelocity;

        yield return new WaitForSeconds(dashDuration * 1.8f); // Longer!

        isDashing = false;
        isInvincible = false;
        dashAttackCoroutine = null;
    }

    protected override void HandleMovement()
    {
        if (!canMove) 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);  // ← FORCE zero horizontal every frame
            return;
        }
        
        float targetVelocityX = xInput * speed;
        
        if (transform.position.x <= MIN_PLAYER_X && xInput < 0)
            targetVelocityX = 0;
        
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }
    public virtual void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    //Draw Gizmo method 
    protected override void OnDrawGizmosSelected()
    {
       
        if (TryGetComponent<PlayerCombat>(out var combat))
        {
            combat.DrawAttackGizmos();  
        }
    }
    
}