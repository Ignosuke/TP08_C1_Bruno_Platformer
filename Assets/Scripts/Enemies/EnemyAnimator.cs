using UnityEngine;

// Arma la FSM de animaciones del enemigo y la actualiza cada frame, como PlayerAnimator.
// El Animator Controller no tiene transiciones, solo los estados con estos mismos nombres
[RequireComponent(typeof(EnemyBrain), typeof(EnemyHealth))]
public class EnemyAnimator : MonoBehaviour
{
    private const float MoveThreshold = 0.1f;          // Velocidad minima para caminar

    [SerializeField] protected Animator animator;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyBrain brain;
    private EnemyAttack attack;
    private EnemyMover mover;

    private StateMachine fsm;
    protected AnimState idleState;
    protected AnimState walkState;
    protected AnimState hurtState;
    protected AnimState attackState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
        mover = GetComponent<EnemyMover>();
        if (attack == null) Debug.LogError(name + " needs an EnemyAttack component", this);
        if (mover == null) Debug.LogError(name + " needs an EnemyMover component", this);

        BuildStateMachine();
    }

    // Todas son globales porque ninguna depende del estado anterior, el orden es la prioridad
    private void BuildStateMachine()
    {
        fsm = new StateMachine();

        hurtState = new AnimState(animator, "Hurt");
        attackState = new AnimState(animator, "Attack");
        AnimState death = new AnimState(animator, "Death");
        AnimState dash = new AnimState(animator, "Dash");
        walkState = new AnimState(animator, "Walk");
        idleState = new AnimState(animator, "Idle");

        fsm.AddAnyTransition(death, () => health.GetIsDead());
        AddPriorityTransitions(fsm);
        fsm.AddAnyTransition(hurtState, () => health.GetIsHurt() || brain.GetIsStunned());
        fsm.AddAnyTransition(attackState, () => attack.GetIsAttacking() || brain.GetIsRecoiling());   // En el choque sostiene el golpe
        fsm.AddAnyTransition(dash, () => mover.GetIsDashing());     // Solo el arquero tiene Dash
        fsm.AddAnyTransition(walkState, () => IsMoving());
        fsm.AddAnyTransition(idleState, () => !IsMoving());
    }

    // Estados que le ganan a todo salvo a la muerte, por defecto ninguno
    protected virtual void AddPriorityTransitions(StateMachine machine) { }

    protected virtual void OnEnable()
    {
        fsm.ChangeState(null);   // Al volver a prenderse arranca de cero
        health.OnDamaged += PlayHurt;
        brain.OnStunned += PlayStun;
        attack.OnAttackStarted += PlayAttack;
    }

    protected virtual void OnDisable()
    {
        health.OnDamaged -= PlayHurt;
        brain.OnStunned -= PlayStun;
        attack.OnAttackStarted -= PlayAttack;
    }

    private void Update()
    {
        if (HasAnimator()) fsm.Update();
    }

    private bool IsMoving()
    {
        return Mathf.Abs(rb.linearVelocity.x) > MoveThreshold;
    }

    // Cada golpe arranca Hurt de nuevo, con el golpe que mata la FSM elige Death. Con el ataque comprometido sigue atacando
    private void PlayHurt(DamageInfo info)
    {
        if (HasAnimator() && !health.GetIsDead() && !attack.GetIsArmored()) fsm.ChangeState(hurtState);
    }

    private void PlayStun()
    {
        if (HasAnimator()) fsm.ChangeState(hurtState);
    }

    // Cada ataque tiene su clip (Attack, Attack1, Attack2), se elige antes de entrar
    private void PlayAttack()
    {
        attackState.SetStateName(attack.GetCurrentAttack().GetStateName());
        if (HasAnimator()) fsm.ChangeState(attackState);
    }

    private bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
