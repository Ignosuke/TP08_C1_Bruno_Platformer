using UnityEngine;

// Elige que animacion se ve segun el estado de los demas componentes y la reproduce con Play.
// El Animator Controller no tiene transiciones, solo los estados con estos mismos nombres.
// Sin Animator o sin controller asignado no hace nada.
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;      // Se da vuelta segun hacia donde mira

    private const string IdleState = "Idle";
    private const string RunState = "Run";
    private const string DefendState = "Defend";
    private const string JumpStartState = "JumpStart";
    private const string JumpRiseState = "JumpRise";
    private const string JumpApexState = "JumpApex";
    private const string JumpFallState = "JumpFall";
    private const string DashState = "Dash";
    private const string AirAttackState = "AirAttack";

    private const float MoveThreshold = 0.1f;       // Velocidad minima para correr / para considerar que sube
    private const float ApexThreshold = 1f;         // Debajo de esta velocidad de subida empieza el punto alto

    private Rigidbody2D rb;
    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerJumper jumper;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;

    private string currentState;
    private string attackState;         // Que ataque mostrar mientras dura el golpe

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        mover = GetComponent<PlayerMover>();
        jumper = GetComponent<PlayerJumper>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
    }

    private void OnEnable()
    {
        jumper.OnJumped += PlayJump;
        dasher.OnDashed += PlayDash;
        attacker.OnAttacked += PlayAttack;
        attacker.OnAirAttacked += PlayAirAttack;
    }

    private void OnDisable()
    {
        jumper.OnJumped -= PlayJump;
        dasher.OnDashed -= PlayDash;
        attacker.OnAttacked -= PlayAttack;
        attacker.OnAirAttacked -= PlayAirAttack;
    }

    private void Update()
    {
        visualRoot.localScale = new Vector3(mover.GetFacing(), 1f, 1f);

        if (!HasAnimator()) return;
        PlayState(ChooseState(), false);
    }

    // El orden es la prioridad, dash, ataque, defensa, suelo y aire
    private string ChooseState()
    {
        if (dasher.GetIsDashing()) return DashState;
        if (attacker.GetIsAttacking()) return attackState;
        if (defender.GetIsDefending()) return DefendState;

        float verticalSpeed = rb.linearVelocity.y;

        // Al despegar el ground checker todavia marca suelo un instante, por eso tambien pide que no este subiendo
        if (groundChecker.GetIsGrounded() && verticalSpeed < MoveThreshold)
            return ChooseGroundState();

        return ChooseAirState(verticalSpeed);
    }

    private string ChooseGroundState()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > MoveThreshold) return RunState;

        // En la ventana del combo se queda en el ultimo frame del golpe, asi el cuerpo no salta a Idle y vuelve
        if (attacker.GetIsComboWindowOpen() && currentState == attackState) return attackState;

        return IdleState;
    }

    private string ChooseAirState(float verticalSpeed)
    {
        // El arranque y el punto alto se ven enteros antes de seguir
        if ((currentState == JumpStartState || currentState == JumpApexState) && !IsCurrentFinished()) return currentState;
        if (verticalSpeed > ApexThreshold) return JumpRiseState;
        if (currentState == JumpRiseState) return JumpApexState;
        return JumpFallState;
    }

    private void PlayJump()
    {
        // Si salta atacando o en dash se sigue viendo esa animacion
        if (!HasAnimator() || dasher.GetIsDashing() || attacker.GetIsAttacking()) return;
        PlayState(JumpStartState, true);
    }

    private void PlayDash()
    {
        if (HasAnimator()) PlayState(DashState, true);
    }

    private void PlayAttack(int step)
    {
        attackState = "Attack" + step;
        if (HasAnimator()) PlayState(attackState, true);
    }

    private void PlayAirAttack()
    {
        attackState = AirAttackState;
        if (HasAnimator()) PlayState(attackState, true);
    }

    // restart = true la arranca de cero aunque ya se este viendo (ej. Attack1 dos veces seguidas)
    private void PlayState(string state, bool restart)
    {
        if (state == currentState && !restart) return;

        currentState = state;
        animator.Play(state, 0, 0f);
    }

    private bool IsCurrentFinished()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(currentState) && info.normalizedTime >= 1f;
    }

    private bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
