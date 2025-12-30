using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyCombat combat;

    private void Awake()
    {
        combat = GetComponentInParent<EnemyCombat>();
    }

    // Animation Event
    public void EnemyPerformAttack()
    {
        combat.PerformAttack();
    }

    // Animation Event
    public void EnemyEndAttack()
    {
        combat.EndAttack();
    }
}
