using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference moveAction;
    private Rigidbody2D rb;

    private float moveSpeed;
    private float acceleration;
    private float deceleration;

    private Vector2 moveDir;
    private int facing = 1;             // 1 derecha, -1 izquierda

    private PlayerGroundChecker groundChecker;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;
    private PlayerHealer healer;

    private void Awake()
    {
        moveAction = playerData.GetMoveAction();
        moveSpeed = playerData.GetMoveSpeed();
        acceleration = playerData.GetAcceleration();
        deceleration = playerData.GetDeceleration();

        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        moveDir = moveAction.action.ReadValue<Vector2>();

        // Mientras ataca, defiende, dashea o se cura mantiene la direccion en la que empezo
        bool canTurn = !attacker.GetIsAttacking() && !defender.GetIsDefending() && !dasher.GetIsDashing() && !healer.GetIsHealing();
        if (canTurn && moveDir.x > 0.1f) facing = 1;
        if (canTurn && moveDir.x < -0.1f) facing = -1;
    }

    private void FixedUpdate()
    {
        // Durante el dash la velocidad la maneja el dasher
        if (dasher.GetIsDashing()) return;

        float targetSpeed = moveDir.x * moveSpeed;

        // Defendiendo, curandose o atacando en el suelo se queda quieto; en el aire puede seguir moviendose
        if (defender.GetIsDefending() || healer.GetIsHealing() || (attacker.GetIsAttacking() && groundChecker.GetIsGrounded()))
            targetSpeed = 0f;

        // Acelera y frena de a poco para que se sienta con peso pero responda rapido
        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }

    public int GetFacing()
    {
        return facing;
    }
}
