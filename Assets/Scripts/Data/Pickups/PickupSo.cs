using UnityEngine;

// Los valores de las monedas y los pickables del nivel, uno solo para todos
[CreateAssetMenu(fileName = "PickupSo", menuName = "Data/Pickup")]
public class PickupSo : ScriptableObject
{
    [Header("Coins")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private int pouchValue = 5;

    [Header("Enemy Drop")]
    [Tooltip("Max sideways impulse of each dropped coin, random between minus and plus this value")]
    [SerializeField] private float dropSideImpulse = 2.5f;
    [SerializeField] private float dropMinUpImpulse = 5f;
    [SerializeField] private float dropMaxUpImpulse = 8f;
    [Tooltip("Height above the enemy feet where the coins spawn")]
    [SerializeField] private float dropHeight = 0.8f;

    [Header("Heal")]
    [SerializeField] private int healAmount = 2;                // En medios corazones, se consume aunque la vida este llena

    [Header("Damage Buff")]
    [SerializeField] private int damageBonus = 1;               // Se suma a todos los golpes, combo y aereo
    [SerializeField] private float damageDuration = 8f;         // Otro pickable reinicia el tiempo, no se acumula
    [SerializeField] private Color damageTint = new Color(1f, 0.55f, 0.55f);

    [Header("Invulnerability")]
    [SerializeField] private float invulnerableDuration = 4f;
    [Tooltip("Wins over the damage tint when both are active")]
    [SerializeField] private Color invulnerableTint = new Color(0.55f, 0.9f, 1f);
    [Tooltip("Seconds of blinking at the start, the pickup flash")]
    [SerializeField] private float invulnerableFlashTime = 0.3f;
    [Tooltip("Seconds of blinking before it ends, the warning")]
    [SerializeField] private float invulnerableWarningTime = 1.2f;
    [SerializeField] private float blinkInterval = 0.08f;

    [Header("Sounds")]
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip pouchClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip invulnerabilityClip;

    public int GetCoinValue() => coinValue;
    public int GetPouchValue() => pouchValue;

    public float GetDropSideImpulse() => dropSideImpulse;
    public float GetDropMinUpImpulse() => dropMinUpImpulse;
    public float GetDropMaxUpImpulse() => dropMaxUpImpulse;
    public float GetDropHeight() => dropHeight;

    public int GetHealAmount() => healAmount;

    public int GetDamageBonus() => damageBonus;
    public float GetDamageDuration() => damageDuration;
    public Color GetDamageTint() => damageTint;

    public float GetInvulnerableDuration() => invulnerableDuration;
    public Color GetInvulnerableTint() => invulnerableTint;
    public float GetInvulnerableFlashTime() => invulnerableFlashTime;
    public float GetInvulnerableWarningTime() => invulnerableWarningTime;
    public float GetBlinkInterval() => blinkInterval;

    public AudioClip GetCoinClip() => coinClip;
    public AudioClip GetPouchClip() => pouchClip;
    public AudioClip GetHealClip() => healClip;
    public AudioClip GetDamageClip() => damageClip;
    public AudioClip GetInvulnerabilityClip() => invulnerabilityClip;
}
