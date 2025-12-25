using UnityEngine;

public class Player_KaAnimationEvents : MonoBehaviour
{
    private PlayerScript player;
    
    public void Awake()
    {
        player = GetComponentInParent<PlayerScript>();//Assuming Player script is in parent GameObject .assumption was right
    }

    public void TriggerAttackDamage()
    {
        player.DamageTarget();
    }

    public void DisableMovementAndJump()
    {
        player.EnableJumpandMovement(false);
    }

    public void EnableMovementAndJump()
    {
        player.EnableJumpandMovement(true);
    }
}
