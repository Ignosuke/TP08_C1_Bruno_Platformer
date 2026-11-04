using UnityEngine;

// Arma la FSM de animaciones del enemigo y la actualiza cada frame, como PlayerAnimator.
// El Animator Controller no tiene transiciones, solo los estados con estos mismos nombres
[RequireComponent(typeof(EnemyBrain))]
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const float MoveThreshold = 0.1f;          // Velocidad minima para caminar

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyBrain brain;
    private EnemyAttack attack;

    private StateMachine fsm;
    private AnimState hurtState;
    private AnimState attackState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();

        BuildStateMachine();
    }

    // Todas son globales porque ninguna depende del estado anterior, el orden es la prioridad
    private void BuildStateMachine()
    {
        fsm = new StateMachine();

        hurtState = new AnimState(animator, "Hurt");
        attackState = new AnimState(animator, "Attack");
        AnimState death = new AnimState(animator, "Death");
        AnimState walk = new AnimState(animator, "Walk");
        AnimState idle = new AnimState(animator, "Idle");

        fsm.AddAnyTransition(death, () => health.GetIsDead());
        fsm.AddAnyTransition(hurtState, () => health.GetIsHurt() || brain.GetIsStunned());
        fsm.AddAnyTransition(attackState, () => attack.GetIsAttacking() || brain.GetIsRecoiling());   // En el choque sostiene el golpe
        fsm.AddAnyTransition(walk, () => IsMoving());
        fsm.AddAnyTransition(idle, () => !IsMoving());
    }

    private void OnEnable()
    {
        fsm.ChangeState(null);   // Al volver a prenderse arranca de cero
        health.OnDamaged += PlayHurt;
        brain.OnStunned += PlayStun;
        attack.OnAttackStarted += PlayAttack;
    }

    private void OnDisable()
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
