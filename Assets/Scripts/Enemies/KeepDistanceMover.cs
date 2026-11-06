using UnityEngine;

// Patrulla como PatrolChaseMover y cuando ve al jugador se queda a su distancia preferida para disparar.
// Si el jugador se acerca demasiado sale con un dash para atras, salvo que atras haya un borde o una pared.
// Acorralado se queda mirando al jugador y le dispara
[RequireComponent(typeof(EnemySight))]
public class KeepDistanceMover : EnemyMover
{
    private const float DistanceTolerance = 0.5f;   // Margen alrededor de la distancia preferida, asi no tiembla

    private EnemySight sight;
    private float dashCounter;
    private float nextDashTime;

    protected override void Awake()
    {
        base.Awake();
        sight = GetComponent<EnemySight>();
    }

    // Un golpe, un ataque o el reinicio cortan el dash
    protected override void OnDisable()
    {
        base.OnDisable();
        dashCounter = 0f;
    }

    protected override void UpdateMovement()
    {
        if (GetIsDashing())
        {
            Dash();
            return;
        }

        if (!sight.CanSeePlayer())
        {
            Patrol(enemyData.GetMoveSpeed());
            return;
        }

        float distance = sight.GetPlayerDistanceX();
        float gap = Mathf.Abs(distance);
        if (CanDashAway(distance)) StartDash(distance);
        else if (gap < enemyData.GetPreferredDistance() - DistanceTolerance) Retreat(distance);
        else if (gap > enemyData.GetPreferredDistance() + DistanceTolerance) Approach(distance);
        else Hold(distance);
    }

    // Camina para atras dando la espalda, se da vuelta recien para disparar
    private void Retreat(float distance)
    {
        int back = AwayFrom(distance);
        if (IsBlocked(back))
        {
            Hold(distance);
            return;
        }

        SetDirection(back);
        Move(enemyData.GetChaseSpeed());
    }

    // No baja bordes, asi no se cae del lugar desde donde dispara
    private void Approach(float distance)
    {
        FaceTowards(distance);
        if (IsBlocked()) Stop();
        else Move(enemyData.GetChaseSpeed());
    }

    private void Hold(float distance)
    {
        FaceTowards(distance);
        Stop();
    }

    private bool CanDashAway(float distance)
    {
        return Mathf.Abs(distance) < enemyData.GetFleeDistance() && Time.time >= nextDashTime && !IsBlocked(AwayFrom(distance));
    }

    private void StartDash(float distance)
    {
        SetDirection(AwayFrom(distance));
        dashCounter = enemyData.GetDashDuration();
        nextDashTime = Time.time + enemyData.GetDashCooldown();
        Dash();
    }

    // Termina antes si llega a un borde o una pared
    private void Dash()
    {
        dashCounter -= Time.fixedDeltaTime;
        if (IsBlocked()) dashCounter = 0f;

        if (GetIsDashing()) Move(enemyData.GetDashSpeed());
        else Stop();
    }

    private static int AwayFrom(float distance) => distance > 0f ? -1 : 1;

    public override bool GetIsDashing() => dashCounter > 0f;
    public override bool GetIsEvading() => GetIsDashing() || (sight.CanSeePlayer() && CanDashAway(sight.GetPlayerDistanceX()));
}
