using UnityEngine;

// Persigue al jugador durante la pelea, mas rapido en la segunda fase, y se frena a la distancia del proximo ataque.
// Antes de la pelea y en el cambio de fase se queda quieto mirandolo
[RequireComponent(typeof(EnemySight), typeof(BossPhase))]
public class BossMover : EnemyMover
{
    private EnemySight sight;
    private BossPhase phase;
    private BossBrain brain;
    private BossAttack attack;

    protected override void Awake()
    {
        base.Awake();
        sight = GetComponent<EnemySight>();
        phase = GetComponent<BossPhase>();
        brain = GetComponent<BossBrain>();
        attack = GetComponent<BossAttack>();
        if (brain == null || attack == null) Debug.LogError(name + " needs BossBrain and BossAttack components", this);
    }

    protected override void UpdateMovement()
    {
        float distance = sight.GetPlayerDistanceX();
        FaceTowards(distance);

        bool isWaiting = !brain.GetIsFighting() || phase.GetIsHoldingStill();
        if (isWaiting || Mathf.Abs(distance) <= attack.GetNextRange() || HasWallAhead())
            Stop();
        else
            Move(enemyData.GetChaseSpeed() * phase.GetSpeedMultiplier());
    }
}
