using System.Collections.Generic;
using UnityEngine;

// Los valores del jefe. Lo comun con los enemigos esta en EnemySo y es la primera fase,
// aca va lo que cambia en la segunda, los patrones de ataque, el salto y el cambio de fase
[CreateAssetMenu(fileName = "BossSo", menuName = "Data/Boss")]
public class BossSo : EnemySo
{
    [Header("Phases")]
    [Tooltip("Health of the second phase, taken from Max Health. The first phase gets the rest")]
    [SerializeField] private int phaseTwoHealth = 15;
    [Tooltip("Chase speed multiplier in the second phase")]
    [SerializeField] private float phaseTwoSpeedMultiplier = 1.35f;
    [Tooltip("Wait between patterns in the second phase. The first phase uses Attack Cooldown")]
    [SerializeField] private float phaseTwoPatternCooldown = 0.6f;
    [Tooltip("Stun after a perfect parry in the second phase. The first phase uses Stun Duration")]
    [SerializeField] private float phaseTwoStunDuration = 0.75f;

    [Header("Patterns")]
    [Tooltip("Picked at random, never the same one twice in a row if there is another")]
    [SerializeField] private List<BossPattern> patterns = new List<BossPattern>();
    [Tooltip("Second phase attacks, like Attacks but with the Flaming suffix in State Name")]
    [SerializeField] private List<EnemyAttackData> flamingAttacks = new List<EnemyAttackData>();

    [Header("Leap")]
    [Tooltip("State name of the attack that leaps toward the player, without the Flaming suffix")]
    [SerializeField] private string leapAttackName = "JumpAttack";
    [Tooltip("Max distance to the player to start the leap")]
    [SerializeField] private float leapRange = 5f;
    [Tooltip("Distance in front of the player where the leap and the lunges stop")]
    [SerializeField] private float leapLandDistance = 1.1f;

    [Header("Chain")]
    [Tooltip("Once a pattern started, the next steps start from this far. Larger than Attack Range so knockback does not break the chain")]
    [SerializeField] private float chainRange = 4f;
    [Tooltip("Speed of the step toward the player during each slash, until it is Leap Land Distance away or the hit lands")]
    [SerializeField] private float lungeSpeed = 9f;

    [Header("Phase Change")]
    [SerializeField] private float shoutDuration = 1.4f;        // Lo que dura la animacion Shout
    [Tooltip("Seconds the boss bar takes to refill for the second phase")]
    [SerializeField] private float regenDuration = 1f;
    [Tooltip("Seconds the boss stands still after the phase change before it moves or attacks again")]
    [SerializeField] private float phaseTwoStartDelay = 1.5f;

    [Header("Boss Sounds")]
    [SerializeField] private AudioClip shoutClip;

    public int GetPhaseTwoHealth() => phaseTwoHealth;
    public float GetPhaseTwoSpeedMultiplier() => phaseTwoSpeedMultiplier;
    public float GetPhaseTwoPatternCooldown() => phaseTwoPatternCooldown;
    public float GetPhaseTwoStunDuration() => phaseTwoStunDuration;

    public int GetPatternCount() => patterns.Count;
    public BossPattern GetPattern(int index) => patterns[index];

    public string GetLeapAttackName() => leapAttackName;
    public float GetLeapRange() => leapRange;
    public float GetLeapLandDistance() => leapLandDistance;

    public float GetChainRange() => chainRange;
    public float GetLungeSpeed() => lungeSpeed;

    public float GetShoutDuration() => shoutDuration;
    public float GetRegenDuration() => regenDuration;
    public float GetPhaseTwoStartDelay() => phaseTwoStartDelay;

    public AudioClip GetShoutClip() => shoutClip;

    // Busca en los ataques de las dos fases, null si ninguno tiene ese estado
    public EnemyAttackData FindAttack(string stateName)
    {
        for (int i = 0; i < GetAttackCount(); i++)
        {
            if (GetAttack(i).GetStateName() == stateName) return GetAttack(i);
        }
        foreach (EnemyAttackData attack in flamingAttacks)
        {
            if (attack.GetStateName() == stateName) return attack;
        }
        return null;
    }
}
