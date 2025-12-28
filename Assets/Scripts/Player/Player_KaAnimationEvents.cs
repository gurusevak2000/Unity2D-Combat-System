using UnityEngine;

public class Player_KaAnimationEvents : MonoBehaviour
{
    private PlayerCombat playerCombat;  // CHANGED: now gets PlayerCombat, not PlayerScript
    
    public void Awake()
    {
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
        playerCombat.DisableMovementAndJump();  // Calls the new method in PlayerCombat
    }

    public void EnableMovementAndJump()
    {
        playerCombat.EnableMovementAndJump();   // Calls the new method in PlayerCombat
    }
}