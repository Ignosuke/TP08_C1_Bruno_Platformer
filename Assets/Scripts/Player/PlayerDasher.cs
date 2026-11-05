using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDasher : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference dashAction;

    private float dashSpeed;
    private float dashDuration;
    private float dashCooldown;

    private float dashCounter;
    private float cooldownCounter;

    private Rigidbody2D rb;
    private PlayerMover mover;
    private PlayerDefender defender;
    private PlayerHealer healer;

    private bool dashRequested;
    private bool isDashing;
    private int dashDirection;

    public event Action OnDashed;

    private void Awake()
    {
        dashAction = playerData.GetDashAction();
        dashSpeed = playerData.GetDashSpeed();
        dashDuration = playerData.GetDashDuration();
        dashCooldown = playerData.GetDashCooldown();

        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<PlayerMover>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
    }

    private void OnEnable()
    {
        dashAction.action.performed += OnDashPerformed;
        dashAction.action.Enable();
    }

    private void OnDisable()
    {
        dashAction.action.performed -= OnDashPerformed;
        dashAction.action.Disable();
        isDashing = false;
        dashRequested = false;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashRequested = true;
    }

    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;

        if (isDashing)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0f) StopDash();
        }

        if (dashRequested)
        {
            dashRequested = false;
            if (!isDashing && cooldownCounter <= 0f && !defender.GetIsDefending() && !healer.GetIsHealing()) StartDash();
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing) return;

        // Velocidad fija y sin caer, el dash siempre recorre la misma distancia
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
    }

    private void StartDash()
    {
        isDashing = true;
        dashCounter = dashDuration;
        cooldownCounter = dashCooldown;
        dashDirection = mover.GetFacing();
        OnDashed?.Invoke();
    }

    // Frena en seco al terminar; el mover vuelve a acelerar si se sigue apretando
    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }

    public bool GetIsDashing() => isDashing;
}
