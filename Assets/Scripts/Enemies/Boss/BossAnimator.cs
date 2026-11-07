using UnityEngine;

// Suma al animador del enemigo el grito del cambio de fase y los clips de la espada en llamas.
// Los ataques ya traen su nombre de la fase desde el BossSo, aca se cambian los demas
[RequireComponent(typeof(BossPhase))]
public class BossAnimator : EnemyAnimator
{
    private const string IdleName = "Idle";
    private const string WalkName = "Walk";
    private const string HurtName = "Hurt";
    private const string ShoutName = "Shout";

    private BossPhase phase;

    // La fase va antes porque la FSM se arma en el Awake de la base
    protected override void Awake()
    {
        phase = GetComponent<BossPhase>();
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        phase.OnPhaseChanged += RenameStates;
        RenameStates();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        phase.OnPhaseChanged -= RenameStates;
    }

    // Mientras grita no hace otra cosa, solo la muerte le gana
    protected override void AddPriorityTransitions(StateMachine machine)
    {
        machine.AddAnyTransition(new AnimState(animator, ShoutName), () => phase.GetIsShouting());
    }

    // Toma efecto la proxima vez que entra a cada estado, al terminar el grito entra a Idle de la fase nueva
    private void RenameStates()
    {
        idleState.SetStateName(phase.GetStateName(IdleName));
        walkState.SetStateName(phase.GetStateName(WalkName));
        hurtState.SetStateName(phase.GetStateName(HurtName));
    }
}
