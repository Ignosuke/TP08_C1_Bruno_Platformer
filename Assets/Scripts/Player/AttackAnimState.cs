using UnityEngine;

// Un solo estado para todos los golpes, el clip sale del paso del combo o del ataque aereo
public class AttackAnimState : AnimState
{
    private const string AirAttackName = "AirAttack";

    public AttackAnimState(Animator animator) : base(animator, "Attack1") { }

    public void SetComboStep(int step)
    {
        stateName = "Attack" + step;
    }

    public void SetAirAttack()
    {
        stateName = AirAttackName;
    }

    public bool GetIsAirAttack() => stateName == AirAttackName;
}
