using UnityEngine;

// Patrulla y persigue al jugador cuando lo ve. Lo usan el Oni y el Executioner, cambia solo el EnemySo
[RequireComponent(typeof(EnemySight))]
public class PatrolChaseMover : EnemyMover
{
    private EnemySight sight;

    protected override void Awake()
    {
        base.Awake();
        sight = GetComponent<EnemySight>();
    }

    protected override void UpdateMovement()
    {
        if (!sight.CanSeePlayer())
        {
            Patrol(enemyData.GetMoveSpeed());
            return;
        }

        float distance = sight.GetPlayerDistanceX();
        FaceTowards(distance);

        // Se frena al alcance del ataque o en un borde, asi no se tira persiguiendo
        if (Mathf.Abs(distance) <= enemyData.GetAttackRange() || IsBlocked())
            Stop();
        else
            Move(enemyData.GetChaseSpeed());
    }
}
