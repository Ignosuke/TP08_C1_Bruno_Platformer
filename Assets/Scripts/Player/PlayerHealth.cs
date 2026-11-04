using System;
using UnityEngine;

// La vida del jugador. Suma a Health el bloqueo y el parry de frente y la reaparicion al morir.
// La reaparicion la hace PlayerRespawner, que despues llama a Revive
public class PlayerHealth : Health
{
    [SerializeField] private PlayerSo playerData;

    private PlayerMover mover;
    private PlayerDefender defender;
    private PlayerRespawner respawner;

    public event Action OnRespawned;                 // Para la camara, los enemigos y las monedas de la historia 006
    public event Action<DamageInfo> OnParried;       // Parry perfecto, PlayerAttacker contraataca si fue cuerpo a cuerpo

    public override int MaxHealth => playerData.GetMaxHealth();
    protected override float InvulnerableTime => playerData.GetInvulnerableTime();
    protected override float HurtDuration => playerData.GetHurtDuration();
    protected override LayerMask DamageLayers => playerData.GetDamageLayers();

    protected override void Awake()
    {
        base.Awake();
        mover = GetComponent<PlayerMover>();
        defender = GetComponent<PlayerDefender>();
        respawner = GetComponent<PlayerRespawner>();
    }

    // Parry perfecto, no recibe nada y el atacante queda aturdido (los proyectiles ignoran el resultado)
    protected override bool IsParried(DamageInfo info)
    {
        bool parried = GetIsBlocking(info) && defender.GetIsParrying();
        if (parried) OnParried?.Invoke(info);
        return parried;
    }

    // Bloqueo fuera de la ventana, redondea para arriba asi un bloqueo nunca saca 0
    protected override int ModifyDamage(DamageInfo info)
    {
        if (!GetIsBlocking(info)) return info.Amount;
        return Mathf.CeilToInt(info.Amount * playerData.GetBlockDamageMultiplier());
    }

    protected override void Die()
    {
        respawner.StartRespawn();
    }

    // Vuelve con la vida llena, lo llama PlayerRespawner despues de mover al jugador
    public void Revive()
    {
        Refill();
        OnRespawned?.Invoke();
    }

    // Solo defiende si mira hacia el golpe, de espaldas recibe todo. PlayerKnockback lo usa para empujar menos
    public bool GetIsBlocking(DamageInfo info)
    {
        return defender.GetIsDefending() && info.Direction.x * mover.GetFacing() < 0f;
    }
}
