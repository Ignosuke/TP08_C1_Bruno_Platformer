using System;
using UnityEngine;

// Suma al cerebro del enemigo la pelea del jefe. Arranca cuando el jugador llega a la zona de pelea,
// no ataca durante el cambio de fase, se aturde lo que diga la fase y avisa cuando lo vencen
[RequireComponent(typeof(BossPhase))]
public class BossBrain : EnemyBrain
{
    [Tooltip("Zone the player has to reach to start the fight, assigned by the builder")]
    [SerializeField] private BoxCollider2D fightZone;

    private BossPhase phase;
    private bool isFighting;

    public event Action OnFightStarted;
    public event Action OnFightReset;   // El jugador reaparecio y la pelea vuelve a empezar de cero
    public event Action OnDefeated;     // Para el final de la partida y el puntaje

    protected override void Awake()
    {
        base.Awake();
        phase = GetComponent<BossPhase>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        health.OnDied += Defeat;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        health.OnDied -= Defeat;
    }

    protected override void Update()
    {
        if (!isFighting && GetIsPlayerInReach())
        {
            isFighting = true;
            OnFightStarted?.Invoke();
        }
        base.Update();
    }

    // Solo la zona, asi espera quieto en el medio aunque lo vea desde la entrada
    private bool GetIsPlayerInReach()
    {
        PlayerHealth player = sight.GetPlayer();
        if (player == null || player.GetIsDead()) return false;
        return fightZone.OverlapPoint(player.transform.position);
    }

    protected override bool GetCanAttack() => isFighting && !phase.GetIsHoldingStill();

    protected override float GetStunDuration() => phase.GetStunDuration();

    // Primero la fase, asi la barra y las animaciones vuelven a la primera antes de prenderse de nuevo
    public override void ResetToSpawn()
    {
        isFighting = false;
        phase.ResetPhase();
        base.ResetToSpawn();
        OnFightReset?.Invoke();
    }

    private void Defeat()
    {
        OnDefeated?.Invoke();
    }

    public bool GetIsFighting() => isFighting;
}
