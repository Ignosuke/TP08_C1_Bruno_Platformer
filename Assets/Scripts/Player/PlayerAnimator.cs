using UnityEngine;

// Arma la FSM de animaciones del jugador y la actualiza cada frame.
// Los demas componentes manejan el juego, aca solo se elige que animacion se ve.
// El Animator Controller no tiene transiciones, solo los estados con estos mismos nombres.
// Sin Animator o sin controller asignado no hace nada.
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;      // Se da vuelta segun hacia donde mira

    private const float MoveThreshold = 0.1f;       // Velocidad minima para correr / para considerar que sube
    private const float ApexThreshold = 1f;         // Debajo de esta velocidad de subida empieza el punto alto

    private Rigidbody2D rb;
    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerJumper jumper;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;
    private PlayerHealth health;
    private PlayerHealer healer;

    private StateMachine fsm;
    private AnimState idle;
    private AnimState jumpStart;
    private AnimState dash;
    private AttackAnimState attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        mover = GetComponent<PlayerMover>();
        jumper = GetComponent<PlayerJumper>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
        health = GetComponent<PlayerHealth>();
        healer = GetComponent<PlayerHealer>();

        BuildStateMachine();
    }

    private void BuildStateMachine()
    {
        fsm = new StateMachine();

        idle = new AnimState(animator, "Idle");
        jumpStart = new AnimState(animator, "JumpStart");
        dash = new AnimState(animator, "Dash");
        attack = new AttackAnimState(animator);
        AnimState run = new AnimState(animator, "Run");
        AnimState jumpRise = new AnimState(animator, "JumpRise");
        AnimState jumpApex = new AnimState(animator, "JumpApex");
        AnimState jumpFall = new AnimState(animator, "JumpFall");
        AnimState defend = new AnimState(animator, "Defend");
        AnimState hurt = new AnimState(animator, "Hurt");
        AnimState death = new AnimState(animator, "Death");
        AnimState healing = new AnimState(animator, "Healing");

        // Globales, el orden es la prioridad, muerte, dano, curacion, dash, ataque y defensa
        fsm.AddAnyTransition(death, () => health.GetIsDead());
        fsm.AddAnyTransition(hurt, () => health.GetIsHurt());
        fsm.AddAnyTransition(healing, () => healer.GetIsHealing());
        fsm.AddAnyTransition(dash, () => dasher.GetIsDashing());
        fsm.AddAnyTransition(attack, () => attacker.GetIsAttacking());
        fsm.AddAnyTransition(defend, () => defender.GetIsDefending());

        // En la ventana del combo se queda en el ultimo frame del golpe, asi el cuerpo no salta a Idle y vuelve.
        // El ataque aereo nunca se sostiene en el piso
        fsm.AddTransition(attack, attack, () => IsOnGround() && !IsRunning() && attacker.GetIsComboWindowOpen() && !attack.GetIsAirAttack());

        // El arranque y el punto alto se ven enteros antes de seguir
        fsm.AddTransition(jumpStart, jumpStart, () => !IsOnGround() && !jumpStart.GetIsFinished());
        fsm.AddTransition(jumpApex, jumpApex, () => !IsOnGround() && !jumpApex.GetIsFinished());

        // Al dejar de subir pasa por el punto alto
        fsm.AddTransition(jumpRise, jumpApex, () => !IsOnGround() && !IsRising());

        // Desde cualquier estado se vuelve al suelo o al aire
        AnimState[] allStates = { idle, run, jumpStart, jumpRise, jumpApex, jumpFall, dash, attack, defend, hurt, death, healing };
        foreach (AnimState from in allStates)
        {
            fsm.AddTransition(from, run, () => IsOnGround() && IsRunning());
            fsm.AddTransition(from, idle, () => IsOnGround() && !IsRunning());
            fsm.AddTransition(from, jumpRise, () => IsRising());
            fsm.AddTransition(from, jumpFall, () => !IsOnGround() && !IsRising());
        }
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

    private void Start()
    {
        if (HasAnimator()) fsm.ChangeState(idle);
    }

    private void Update()
    {
        visualRoot.localScale = new Vector3(mover.GetFacing(), 1f, 1f);

        if (HasAnimator()) fsm.Update();
    }

    // Al despegar el ground checker todavia marca suelo un instante, por eso tambien pide que no este subiendo
    private bool IsOnGround()
    {
        return groundChecker.GetIsGrounded() && rb.linearVelocity.y < MoveThreshold;
    }

    private bool IsRunning()
    {
        return Mathf.Abs(rb.linearVelocity.x) > MoveThreshold;
    }

    private bool IsRising()
    {
        return rb.linearVelocity.y > ApexThreshold;
    }

    // ChangeState siempre arranca de cero, asi Attack1 dos veces seguidas se ve dos veces
    private void PlayJump()
    {
        // Si salta atacando o en dash se sigue viendo esa animacion
        if (!HasAnimator() || dasher.GetIsDashing() || attacker.GetIsAttacking()) return;
        fsm.ChangeState(jumpStart);
    }

    private void PlayDash()
    {
        if (HasAnimator()) fsm.ChangeState(dash);
    }

    private void PlayAttack(int step)
    {
        attack.SetComboStep(step);
        if (HasAnimator()) fsm.ChangeState(attack);
    }

    private void PlayAirAttack()
    {
        attack.SetAirAttack();
        if (HasAnimator()) fsm.ChangeState(attack);
    }

    private bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
