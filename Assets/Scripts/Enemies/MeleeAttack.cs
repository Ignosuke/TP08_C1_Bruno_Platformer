using UnityEngine;

// Golpe cuerpo a cuerpo con MeleeHitbox. Si el jugador hace parry perfecto el atacante queda aturdido
[RequireComponent(typeof(MeleeHitbox))]
public class MeleeAttack : EnemyAttack
{
    private MeleeHitbox hitbox;

    protected override void Awake()
    {
        base.Awake();
        hitbox = GetComponent<MeleeHitbox>();
    }

    protected override bool IsTargetInRange()
    {
        return sight.CanSeePlayer() && Mathf.Abs(sight.GetPlayerDistanceX()) <= enemyData.GetAttackRange();
    }

    protected override void PerformHit()
    {
        bool parried = hitbox.Hit(enemyData.GetDamage(), enemyData.GetAttackKnockback(), enemyData.GetHitboxOffset(),
            enemyData.GetHitboxSize(), enemyData.GetTargetLayer(), mover.GetDirection(),
            enemyData.GetAttackHitStop(), enemyData.GetAttackShakeMagnitude(), enemyData.GetAttackShakeDuration());
        if (parried) Parried();
    }
}
