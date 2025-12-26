using UnityEngine;

public class Player_KaAnimationEvents : MonoBehaviour
{
    private PlayerScript player;
    private PlayerCombat playerCombat;
    
    public void Awake()
    {
        player = GetComponentInParent<PlayerScript>();//Assuming Player script is in parent GameObject .assumption was right
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    public void TriggerAttackDamage()
    {
        playerCombat.PerformAttack();
    }

    // Animation Event
    public void EndAttack()
    {
        playerCombat.EndAttack();
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
