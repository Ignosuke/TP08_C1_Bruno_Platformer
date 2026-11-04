using UnityEngine;

// Un solo estado para todos los golpes, el clip sale del paso del combo o del ataque aereo
public class AttackAnimState : AnimState
{
    private const string AirAttackName = "AirAttack";

    public AttackAnimState(Animator animator) : base(animator, "Attack1") { }

    // stepStartTime mayor a 0 solo en el contraataque del parry
    public void SetComboStep(int step, float stepStartTime)
    {
        stateName = "Attack" + step;
        startTime = stepStartTime;
    }

    public void SetAirAttack()
    {
        stateName = AirAttackName;
        startTime = 0f;
    }

    public bool GetIsAirAttack() => stateName == AirAttackName;
}
