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
        EnemyAttackData current = GetCurrentAttack();
        bool parried = hitbox.Hit(current.GetDamage(), current.GetKnockback(), current.GetHitboxOffset(), current.GetHitboxSize(),
            enemyData.GetTargetLayer(), mover.GetDirection(), current.GetHitStop());
        hitbox.Shake(current.GetShakeMagnitude(), current.GetShakeDuration());   // Pegue o no, el golpe pesado se siente
        if (parried) Parried();
    }
}
