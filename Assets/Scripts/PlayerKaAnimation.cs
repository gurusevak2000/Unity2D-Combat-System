using UnityEngine;

public class PlayerKaAnimation : MonoBehaviour
{
    private PlayerScript player;
    
    public void Awake()
    {
        player = GetComponentInParent<PlayerScript>();//Assuming Player script is in parent GameObject .assumption was right
    }

    public void TriggerAttackDamage()
    {
        player.DamageEnemies();
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
