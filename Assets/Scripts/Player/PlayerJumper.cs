using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJumper : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference jumpAction;

    private float jumpForce;
    private float remainingForceOnTapJump; // Cuanto de la velocidad sobrevive al soltar
    private float riseGravity;          // Mientras sube
    private float fallGravity;          // Apenas empieza a caer
    private float heavyFallGravity;     // Caida acelerada
    private float heavyFallThreshold;   // Velocidad de caida a la que se activa
    private float jumpBufferTime;
    private float jumpLockTime;
    private float coyoteTime;

    private float bufferCounter;
    private float lockCounter;
    private float coyoteCounter;

    private PlayerGroundChecker groundChecker;
    private PlayerDasher dasher;
    private PlayerDefender defender;
    private PlayerHealer healer;
    private Rigidbody2D rb;

    private bool jumpRequested;
    private bool jumpCutRequested;

    public event Action OnJumped;

    private void Awake()
    {
        jumpAction = playerData.GetJumpAction();
        jumpForce = playerData.GetJumpForce();
        remainingForceOnTapJump = playerData.GetRemainingForceOnTapJump();
        riseGravity = playerData.GetRiseGravity();
        fallGravity = playerData.GetFallGravity();
        heavyFallGravity = playerData.GetHeavyFallGravity();
        heavyFallThreshold = playerData.GetHeavyFallThreshold();
        coyoteTime = playerData.GetCoyoteTime();
        jumpBufferTime = playerData.GetJumpBufferTime();
        jumpLockTime = playerData.GetJumpLockTime();

        if (groundChecker == null) groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        dasher = GetComponent<PlayerDasher>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        jumpAction.action.performed += OnJumpPerformed;
        jumpAction.action.canceled += OnJumpCanceled;
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPerformed;
        jumpAction.action.canceled -= OnJumpCanceled;
        jumpAction.action.Disable();
        ResetCounters();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        jumpCutRequested = true;
    }

    private void Update()
    {
        if (lockCounter > 0f) lockCounter -= Time.deltaTime;
        UpdateCoyote();
        UpdateBuffer();
    }

    private void FixedUpdate()
    {
        // El dash es horizontal, sin gravedad; el salto queda en el buffer para despues
        if (dasher.GetIsDashing())
        {
            rb.gravityScale = 0f;
            return;
        }

        ApplyGravity();
        HandleJump();       // Primero el salto, si el tap fue muy rapido el corte se aplica al salto recien hecho
        HandleTapJump();
    }

    private void HandleJump()
    {
        if (bufferCounter <= 0f || coyoteCounter <= 0f) return;
        if (defender.GetIsDefending() || healer.GetIsHealing()) return;

        Jump();
    }

    private void Jump()
    {
        bufferCounter = 0f;
        coyoteCounter = 0f;             // Consumimos el coyote para que no entren dos saltos seguidos
        lockCounter = jumpLockTime;

        // Con masa 1 es igual que un impulso, pero la velocidad queda puesta ya y el corte del tap la puede leer en este mismo paso
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce / rb.mass);
        OnJumped?.Invoke();
    }

    // Al soltar la tecla durante la subida se corta parte de la velocidad, tap = salto bajo, hold = salto completo
    private void HandleTapJump()
    {
        if (!jumpCutRequested) return;
        jumpCutRequested = false;

        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * remainingForceOnTapJump);
    }

    // Gravedad personalizada, primero sube liviano, acelera al bajar y cuando la caida pasa cierta velocidad acelera de golpe
    private void ApplyGravity()
    {
        float verticalSpeed = rb.linearVelocity.y;

        if (verticalSpeed > 0f)
            rb.gravityScale = riseGravity;
        else if (verticalSpeed > -heavyFallThreshold)
            rb.gravityScale = fallGravity;
        else
            rb.gravityScale = heavyFallGravity;
    }

    // El coyote se recarga pisando suelo, salvo en los primeros frames despues de saltar
    private void UpdateCoyote()
    {
        if (groundChecker.GetIsGrounded() && lockCounter <= 0f)
            coyoteCounter = coyoteTime;
        else if (coyoteCounter > 0f)
            coyoteCounter -= Time.deltaTime;
    }

    // El buffer guarda el input unos ms por si todavia no podemos saltar
    private void UpdateBuffer()
    {
        if (jumpRequested)
        {
            jumpRequested = false;
            bufferCounter = jumpBufferTime;
            return;
        }

        if (bufferCounter > 0f) bufferCounter -= Time.deltaTime;
    }

    private void ResetCounters()
    {
        jumpRequested = false;
        jumpCutRequested = false;
        coyoteCounter = 0f;
        bufferCounter = 0f;
        lockCounter = 0f;
    }
}
