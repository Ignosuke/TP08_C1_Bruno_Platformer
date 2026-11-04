using System;
using UnityEngine;

// La vida del jugador. Recibe los golpes, avisa cuando duele y cuando muere.
// La reaparicion la hace PlayerRespawner, que despues llama a Revive
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerSo playerData;

    private float invulnerableTime;
    private float hurtDuration;
    private float blockDamageMultiplier;

    private float invulnerableCounter;
    private float hurtCounter;

    private PlayerMover mover;
    private PlayerDefender defender;
    private ResourcePool health;

    public event Action<int, int> OnHealthChanged;   // Manda la vida actual y la maxima
    public event Action OnHurt;
    public event Action OnDied;
    public event Action OnRespawned;                 // Para las monedas y los enemigos de las historias 004 y 006

    private void Awake()
    {
        invulnerableTime = playerData.GetInvulnerableTime();
        hurtDuration = playerData.GetHurtDuration();
        blockDamageMultiplier = playerData.GetBlockDamageMultiplier();

        mover = GetComponent<PlayerMover>();
        defender = GetComponent<PlayerDefender>();
        health = new ResourcePool(playerData.GetMaxHealth());
    }

    private void Update()
    {
        if (invulnerableCounter > 0f) invulnerableCounter -= Time.deltaTime;
        if (hurtCounter > 0f) hurtCounter -= Time.deltaTime;
    }

    public bool TakeDamage(int amount, Vector2 hitDirection)
    {
        if (health.GetIsEmpty() || invulnerableCounter > 0f) return false;

        // Solo defiende si mira hacia el golpe, de espaldas recibe todo
        bool isFacingHit = hitDirection.x * mover.GetFacing() < 0f;
        if (defender.GetIsDefending() && isFacingHit)
        {
            // Parry perfecto, no recibe nada y el atacante queda aturdido (los proyectiles ignoran el resultado)
            if (defender.GetIsParrying()) return true;

            // Bloqueo fuera de la ventana, redondea para arriba asi un bloqueo nunca saca 0
            amount = Mathf.CeilToInt(amount * blockDamageMultiplier);
        }

        health.Remove(amount);
        invulnerableCounter = invulnerableTime;
        hurtCounter = hurtDuration;

        OnHealthChanged?.Invoke(health.GetCurrent(), health.GetMax());
        OnHurt?.Invoke();

        if (health.GetIsEmpty()) OnDied?.Invoke();
        return false;
    }

    public void Heal(int amount)
    {
        if (health.Add(amount) > 0) OnHealthChanged?.Invoke(health.GetCurrent(), health.GetMax());
    }

    // Vuelve con la vida llena, lo llama PlayerRespawner despues de mover al jugador
    public void Revive()
    {
        health.Refill();
        invulnerableCounter = 0f;
        hurtCounter = 0f;

        OnHealthChanged?.Invoke(health.GetCurrent(), health.GetMax());
        OnRespawned?.Invoke();
    }

    public int GetCurrent() => health.GetCurrent();
    public int GetMax() => health.GetMax();
    public bool GetIsFull() => health.GetIsFull();
    public bool GetIsDead() => health.GetIsEmpty();
    public bool GetIsHurt() => hurtCounter > 0f;
}
