using UnityEngine;

// Escucha los eventos de los demas componentes y se los pasa al Animator.
// Sin Animator o sin controller asignado no hace nada, asi funciona con el placeholder.
// El Animator Controller tiene que tener: Speed y VerticalSpeed (float), Grounded y Defending (bool),
// Jump, Dash, Attack1, Attack2, Attack3 y AirAttack (trigger).
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;      // Se da vuelta segun hacia donde mira

    private Rigidbody2D rb;
    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerJumper jumper;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;

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
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        animator.SetBool("Grounded", groundChecker.GetIsGrounded());
        animator.SetBool("Defending", defender.GetIsDefending());
    }

    private void PlayJump()
    {
        if (HasAnimator()) animator.SetTrigger("Jump");
    }

    private void PlayDash()
    {
        if (HasAnimator()) animator.SetTrigger("Dash");
    }

    private void PlayAttack(int step)
    {
        if (HasAnimator()) animator.SetTrigger("Attack" + step);
    }

    private void PlayAirAttack()
    {
        if (HasAnimator()) animator.SetTrigger("AirAttack");
    }

    private bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }
}
