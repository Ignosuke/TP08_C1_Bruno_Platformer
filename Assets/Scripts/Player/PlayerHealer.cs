using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Curarse gasta una carga al empezar y devuelve vida al terminar la animacion.
// Mientras dura se queda quieto, y si lo golpean se corta y la carga se pierde
[RequireComponent(typeof(PlayerHealth), typeof(PlayerDasher), typeof(PlayerDefender))]
public class PlayerHealer : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference healAction;

    private int healAmount;
    private float healDuration;

    private float healCounter;

    private ResourcePool charges;
    private PlayerHealth health;
    private PlayerGroundChecker groundChecker;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;

    private bool healRequested;
    private bool isHealing;

    public event Action<int, int> OnChargesChanged;   // Manda las cargas actuales y las maximas
    public event Action OnHealStarted;

    private void Awake()
    {
        healAction = playerData.GetHealAction();
        healAmount = playerData.GetHealAmount();
        healDuration = playerData.GetHealDuration();

        charges = new ResourcePool(playerData.GetHealCharges());
        health = GetComponent<PlayerHealth>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
    }

    private void OnEnable()
    {
        healAction.action.performed += OnHealPerformed;
        healAction.action.Enable();
        health.OnDamaged += CancelHeal;
    }

    private void OnDisable()
    {
        healAction.action.performed -= OnHealPerformed;
        healAction.action.Disable();
        health.OnDamaged -= CancelHeal;
        isHealing = false;
        healRequested = false;
    }

    private void OnHealPerformed(InputAction.CallbackContext context)
    {
        healRequested = true;
    }

    private void Update()
    {
        if (isHealing)
        {
            healCounter -= Time.deltaTime;
            if (healCounter <= 0f) FinishHeal();
        }

        if (healRequested)
        {
            healRequested = false;
            if (CanHeal()) StartHeal();
        }
    }

    private bool CanHeal()
    {
        return !isHealing && !charges.GetIsEmpty() && !health.GetIsFull() && groundChecker.GetIsGrounded()
            && !dasher.GetIsDashing() && !attacker.GetIsAttacking() && !defender.GetIsDefending();
    }

    private void StartHeal()
    {
        isHealing = true;
        healCounter = healDuration;
        charges.Remove(1);
        OnChargesChanged?.Invoke(charges.GetCurrent(), charges.GetMax());
        OnHealStarted?.Invoke();
    }

    private void FinishHeal()
    {
        isHealing = false;
        health.Heal(healAmount);
    }

    private void CancelHeal(DamageInfo info)
    {
        isHealing = false;
    }

    // Al reaparecer y al pasar por el campamento del herrero
    public void RefillCharges()
    {
        charges.Refill();
        OnChargesChanged?.Invoke(charges.GetCurrent(), charges.GetMax());
    }

    // Mejora de la tienda, suma la carga ya llena
    public void AddMaxCharges(int amount)
    {
        charges.IncreaseMax(amount);
        OnChargesChanged?.Invoke(charges.GetCurrent(), charges.GetMax());
    }

    public bool GetIsHealing() => isHealing;
    public int GetCharges() => charges.GetCurrent();
    public int GetMaxCharges() => charges.GetMax();
}
