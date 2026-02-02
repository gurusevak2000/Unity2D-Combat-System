using UnityEngine;

public class Player_KaAnimationEvents : MonoBehaviour
{
    private PlayerCombat playerCombat;
    private PlayerScript playerScript;

    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
        playerScript  = GetComponentInParent<PlayerScript>();

        if (playerCombat == null)  Debug.LogError("PlayerCombat not found!", this);
        if (playerScript == null)  Debug.LogError("PlayerScript not found!", this);
    }

    // Only keep what's actually used
    public void TriggerAttackDamage() => playerCombat?.PerformAttack();
    public void EndAttack()           => playerCombat?.EndAttack();

    // Hit events (keep these for later hit animation lock)
    public void LockMovementDuringHit()
    {
        if (playerScript != null)
            playerScript.EnableMovement(false);
        else
            Debug.LogWarning("PlayerScript null in Lock event!", this);
    }

    public void UnlockMovementAfterHit()
    {
        if (playerScript != null)
            playerScript.EnableMovement(true);
        else
            Debug.LogWarning("PlayerScript null in Unlock event!", this);
    }
}