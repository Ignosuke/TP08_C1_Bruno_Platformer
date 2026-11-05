using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerDasher))]
public class PlayerDefender : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference defendAction;

    private float parryWindow;
    private float defendCooldown;

    private float parryCounter;
    private float cooldownCounter;

    private PlayerGroundChecker groundChecker;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerHealer healer;

    private bool defendRequested;
    private bool isHeld;
    private bool isDefending;

    public event Action OnDefendStarted;

    private void Awake()
    {
        defendAction = playerData.GetDefendAction();
        parryWindow = playerData.GetParryWindow();
        defendCooldown = playerData.GetDefendCooldown();

        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        healer = GetComponent<PlayerHealer>();
    }

    private void OnEnable()
    {
        defendAction.action.performed += OnDefendPerformed;
        defendAction.action.canceled += OnDefendCanceled;
        defendAction.action.Enable();
    }

    private void OnDisable()
    {
        defendAction.action.performed -= OnDefendPerformed;
        defendAction.action.canceled -= OnDefendCanceled;
        defendAction.action.Disable();
        isDefending = false;
        isHeld = false;
        defendRequested = false;
    }

    private void OnDefendPerformed(InputAction.CallbackContext context)
    {
        defendRequested = true;
        isHeld = true;
    }

    private void OnDefendCanceled(InputAction.CallbackContext context)
    {
        isHeld = false;
    }

    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;
        if (parryCounter > 0f) parryCounter -= Time.deltaTime;

        if (defendRequested)
        {
            defendRequested = false;
            if (CanDefend()) StartDefend();
        }

        // Aunque se suelte enseguida, la ventana de parry se cumple entera
        if (isDefending && !isHeld && parryCounter <= 0f) StopDefend();
    }

    private bool CanDefend()
    {
        return !isDefending && cooldownCounter <= 0f && groundChecker.GetIsGrounded()
            && !dasher.GetIsDashing() && !attacker.GetIsAttacking() && !healer.GetIsHealing();
    }

    private void StartDefend()
    {
        isDefending = true;
        parryCounter = parryWindow;
        OnDefendStarted?.Invoke();
    }

    // Tambien lo llama PlayerParryCounter en el contraataque del parry
    public void StopDefend()
    {
        isDefending = false;
        cooldownCounter = defendCooldown;
    }

    public bool GetIsDefending() => isDefending;

    // PlayerHealth lo pregunta al recibir un golpe, parry si es true, bloqueo si solo defiende
    public bool GetIsParrying() => isDefending && parryCounter > 0f;
}
