using UnityEngine;

// La vida del jefe, las dos fases en una sola cuenta. En la primera fase no baja de la vida de la segunda
// y lo que sobra del golpe se pierde. Durante el cambio de fase no recibe daño
[RequireComponent(typeof(BossPhase))]
public class BossHealth : EnemyHealth
{
    private BossPhase phase;

    protected override void Awake()
    {
        base.Awake();
        phase = GetComponent<BossPhase>();
    }

    protected override bool GetIsIgnoringHits() => phase.GetIsChanging();

    protected override int ModifyDamage(DamageInfo info)
    {
        if (phase.GetIsPhaseTwo()) return info.Amount;
        return Mathf.Min(info.Amount, GetCurrent() - phase.GetPhaseTwoHealth());
    }
}
