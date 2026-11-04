using UnityEngine;

// Elige que animacion se ve y la reproduce con Play, como PlayerAnimator.
// El Animator Controller no tiene transiciones, solo los estados con estos mismos nombres
[RequireComponent(typeof(EnemyBrain))]
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string IdleState = "Idle";
    private const string WalkState = "Walk";
    private const string AttackState = "Attack";
    private const string HurtState = "Hurt";
    private const string DeathState = "Death";

    private const float MoveThreshold = 0.1f;          // Velocidad minima para caminar

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyBrain brain;
    private EnemyAttack attack;

    private string currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
    }

    private void OnEnable()
    {
        currentState = null;   // Al volver a prenderse arranca de cero
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
        if (HasAnimator()) PlayState(ChooseState(), false);
    }

    // El orden es la prioridad, muerte, dano o aturdido, ataque, caminar y quieto
    private string ChooseState()
    {
        if (health.GetIsDead()) return DeathState;
        if (health.GetIsHurt() || brain.GetIsStunned()) return HurtState;
        if (attack.GetIsAttacking()) return AttackState;
        if (Mathf.Abs(rb.linearVelocity.x) > MoveThreshold) return WalkState;
        return IdleState;
    }

    // Cada golpe arranca Hurt de nuevo, con el golpe que mata Update elige Death. Con el ataque comprometido sigue atacando
    private void PlayHurt(DamageInfo info)
    {
        if (HasAnimator() && !health.GetIsDead() && !attack.GetIsArmored()) PlayState(HurtState, true);
    }

    private void PlayStun()
    {
        if (HasAnimator()) PlayState(HurtState, true);
    }

    private void PlayAttack()
    {
        if (HasAnimator()) PlayState(AttackState, true);
    }

    // restart = true la arranca de cero aunque ya se este viendo
    private void PlayState(string state, bool restart)
    {
        if (state == currentState && !restart) return;

        currentState = state;
        animator.Play(state, 0, 0f);
    }

    private bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
