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

        // Se frena al alcance del ataque o contra una pared. Los bordes los baja, el nivel no tiene pozos
        if (Mathf.Abs(distance) <= enemyData.GetAttackRange() || HasWallAhead())
            Stop();
        else
            Move(enemyData.GetChaseSpeed());
    }
}
