using System;
using UnityEngine;

// Vida compartida por el jugador y los enemigos. Recibe golpes directos con TakeDamage
// y golpes de contacto de los triggers IDamageSource que esten en DamageLayers.
// Cada hijo decide su vida maxima, como se defiende y que pasa al morir
public abstract class Health : MonoBehaviour, IDamageable
{
    private ResourcePool pool;
    private float invulnerableCounter;
    private float hurtCounter;

    public event Action<int, int> OnHealthChanged;     // Manda la vida actual y la maxima
    public event Action<DamageInfo> OnDamaged;         // Solo cuando el golpe saco vida
    public event Action OnDied;

    public abstract int MaxHealth { get; }
    protected abstract float InvulnerableTime { get; } // Despues de un golpe no recibe otro, asi un contacto no saca todo
    protected abstract float HurtDuration { get; }
    protected abstract LayerMask DamageLayers { get; } // De donde vienen los golpes de contacto

    protected virtual void Awake()
    {
        pool = new ResourcePool(MaxHealth);
    }

    protected virtual void Update()
    {
        if (invulnerableCounter > 0f) invulnerableCounter -= Time.deltaTime;
        if (hurtCounter > 0f) hurtCounter -= Time.deltaTime;
    }

    public bool TakeDamage(DamageInfo info)
    {
        if (pool.GetIsEmpty() || invulnerableCounter > 0f) return false;
        if (IsParried(info)) return true;

        pool.Remove(ModifyDamage(info));
        invulnerableCounter = InvulnerableTime;
        hurtCounter = HurtDuration;

        OnHealthChanged?.Invoke(pool.GetCurrent(), pool.GetMax());
        OnDamaged?.Invoke(info);

        if (pool.GetIsEmpty())
        {
            OnDied?.Invoke();
            Die();
        }
        return false;
    }

    // Un parry no saca vida y avisa al atacante, por defecto nadie hace parry
    protected virtual bool IsParried(DamageInfo info) => false;

    // El dano que entra de verdad, por defecto todo
    protected virtual int ModifyDamage(DamageInfo info) => info.Amount;

    protected abstract void Die();

    public void Heal(int amount)
    {
        if (pool.Add(amount) > 0) OnHealthChanged?.Invoke(pool.GetCurrent(), pool.GetMax());
    }

    // Vida llena y sin contadores, para reaparecer o reiniciar
    public void Refill()
    {
        pool.Refill();
        invulnerableCounter = 0f;
        hurtCounter = 0f;
        OnHealthChanged?.Invoke(pool.GetCurrent(), pool.GetMax());
    }

    // Stay y no Enter, asi quedarse pegado vuelve a doler cuando termina la invulnerabilidad
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!DamageLayers.Contains(other.gameObject.layer)) return;
        if (!other.TryGetComponent(out IDamageSource source)) return;

        float side = transform.position.x >= other.bounds.center.x ? 1f : -1f;
        TakeDamage(new DamageInfo(source.Damage, new Vector2(side, 0f), other.gameObject.layer, source.Knockback));
    }

    public int GetCurrent() => pool.GetCurrent();
    public int GetMax() => pool.GetMax();
    public bool GetIsFull() => pool.GetIsFull();
    public bool GetIsDead() => pool.GetIsEmpty();
    public bool GetIsHurt() => hurtCounter > 0f;
}
