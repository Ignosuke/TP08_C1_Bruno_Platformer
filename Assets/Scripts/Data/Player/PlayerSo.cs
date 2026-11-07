using UnityEngine;
using UnityEngine.InputSystem;

// Todos los valores del jugador viven aca para poder tunear sin tocar codigo
[CreateAssetMenu(fileName = "PlayerSo", menuName = "Data/Player")]
public class PlayerSo : ScriptableObject
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference defendAction;
    [SerializeField] private InputActionReference healAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float acceleration = 90f;      // Que tan rapido llega a la velocidad maxima
    [SerializeField] private float deceleration = 120f;     // Que tan rapido frena al soltar

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float remainingForceOnTapJump = 0.45f; // Cuanto de la velocidad sobrevive al soltar
    [SerializeField] private float riseGravity = 3.5f;      // Mientras sube
    [SerializeField] private float fallGravity = 5.5f;      // Apenas empieza a caer
    [SerializeField] private float heavyFallGravity = 8f;   // Caida acelerada
    [SerializeField] private float heavyFallThreshold = 12f; // Velocidad de caida a la que se activa
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpLockTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Air Jump")]
    [Tooltip("Extra jumps in the air per airtime, 1 = double jump, 2 = triple jump")]
    [SerializeField] private int airJumpCount = 1;
    [SerializeField] private float airJumpForce = 13f;      // Pisa la velocidad vertical, no se suma

    [Header("Ground")]
    [SerializeField] private float feetWidth = 0.5f;        // Ancho de la caja de los pies, un poco menos que el collider
    [SerializeField] private float checkDistance = 0.2f;    // Cuanto baja la caja para detectar el suelo
    [SerializeField] private LayerMask groundLayer;         // Solo la capa Ground, los rayos salen de los pies y tocarian al propio jugador

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 24f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;     // Se cuenta desde que arranca el dash

    [Header("Attack")]
    [SerializeField] private int[] comboDamage = { 1, 1, 2 };            // Ataque 1, 2 y 3
    [SerializeField] private float[] comboDuration = { 0.25f, 0.25f, 0.4f };
    [SerializeField] private int airAttackDamage = 1;
    [SerializeField] private float airAttackDuration = 0.3f;
    [Tooltip("Sheet frame (1-based) where each combo hit lands. The builder turns it into an Animation Event")]
    [SerializeField] private int[] comboHitFrames = { 5, 4, 3 };
    [SerializeField] private int airAttackHitFrame = 3;
    [SerializeField] private float comboWindow = 0.3f;      // Tiempo despues de un golpe para encadenar el siguiente
    [SerializeField] private float attackBufferTime = 0.2f;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1.1f, 0f); // Mirando a la derecha
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.6f, 1.3f);
    [SerializeField] private LayerMask hitLayer = ~0;

    [Header("Attack Feel")]
    [Tooltip("Push given to the enemy by each combo hit. Small on 1 and 2 so the next hit still connects")]
    [SerializeField] private float[] comboKnockback = { 1f, 1f, 2.5f };
    [SerializeField] private float airAttackKnockback = 1.5f;
    [Tooltip("Game freeze in real seconds when each combo hit lands, 0 = none")]
    [SerializeField] private float[] comboHitStop = { 0.04f, 0.04f, 0.06f };
    [SerializeField] private float airAttackHitStop = 0.04f;

    [Header("Defend")]
    [SerializeField] private float parryWindow = 0.15f;     // Primeros segundos de la defensa que cuentan como parry
    [SerializeField] private float defendCooldown = 0.25f;  // Para que no se pueda spamear el parry
    [SerializeField] private float blockDamageMultiplier = 0.5f;    // Daño que entra al bloquear fuera del parry, redondeado para arriba
    [Tooltip("Game freeze in real seconds on a perfect parry, the clash")]
    [SerializeField] private float parryHitStop = 0.1f;
    [Tooltip("Attack1 sheet frame (1-based) where the parry counter starts, 5 = the slash")]
    [SerializeField] private int counterStartFrame = 5;

    [Header("Health")]
    [SerializeField] private int maxHealth = 8;             // En medios corazones, 8 = 4 corazones
    [SerializeField] private LayerMask damageLayers;        // Capas que lastiman al tocarlas, como el cuerpo de los demonios
    [SerializeField] private float invulnerableTime = 0.6f; // Despues de un golpe no recibe daño, asi un contacto no saca todo
    [SerializeField] private float hurtDuration = 0.33f;    // Lo que dura la animacion Hurt (4 frames)
    [SerializeField] private float deathDuration = 0.83f;   // Lo que dura la animacion Death (10 frames)
    [SerializeField] private float respawnDelay = 0.6f;     // Tiempo en el suelo despues de la animacion antes de reaparecer

    [Header("Knockback")]
    [Tooltip("Seconds without movement control after being pushed")]
    [SerializeField] private float knockbackDuration = 0.18f;
    [Tooltip("Upward part of the push, as a fraction of the hit strength")]
    [SerializeField] private float knockbackUpward = 0.35f;
    [Tooltip("Push kept when blocking, 0.5 = half")]
    [SerializeField] private float blockKnockbackMultiplier = 0.5f;

    [Header("Heal")]
    [SerializeField] private int healCharges = 3;
    [SerializeField] private int healAmount = 2;            // En medios corazones, 2 = 1 corazon
    [SerializeField] private float healDuration = 1.25f;    // Lo que dura la animacion Healing (15 frames), quieto todo ese tiempo

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip airJumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip[] attackClips = new AudioClip[3];
    [SerializeField] private AudioClip airAttackClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip defendClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip healClip;

    public InputActionReference GetMoveAction() => moveAction;
    public InputActionReference GetJumpAction() => jumpAction;
    public InputActionReference GetAttackAction() => attackAction;
    public InputActionReference GetDashAction() => dashAction;
    public InputActionReference GetDefendAction() => defendAction;
    public InputActionReference GetHealAction() => healAction;

    public float GetMoveSpeed() => moveSpeed;
    public float GetAcceleration() => acceleration;
    public float GetDeceleration() => deceleration;

    public float GetJumpForce() => jumpForce;
    public float GetRemainingForceOnTapJump() => remainingForceOnTapJump;
    public float GetRiseGravity() => riseGravity;
    public float GetFallGravity() => fallGravity;
    public float GetHeavyFallGravity() => heavyFallGravity;
    public float GetHeavyFallThreshold() => heavyFallThreshold;
    public float GetJumpBufferTime() => jumpBufferTime;
    public float GetJumpLockTime() => jumpLockTime;
    public float GetCoyoteTime() => coyoteTime;

    public int GetAirJumpCount() => airJumpCount;
    public float GetAirJumpForce() => airJumpForce;

    public float GetFeetWidth() => feetWidth;
    public float GetCheckDistance() => checkDistance;
    public LayerMask GetGroundLayer() => groundLayer;

    public float GetDashSpeed() => dashSpeed;
    public float GetDashDuration() => dashDuration;
    public float GetDashCooldown() => dashCooldown;

    public int[] GetComboDamage() => comboDamage;
    public float[] GetComboDuration() => comboDuration;
    public int GetAirAttackDamage() => airAttackDamage;
    public float GetAirAttackDuration() => airAttackDuration;
    public int[] GetComboHitFrames() => comboHitFrames;
    public int GetAirAttackHitFrame() => airAttackHitFrame;
    public float GetComboWindow() => comboWindow;
    public float GetAttackBufferTime() => attackBufferTime;
    public Vector2 GetHitboxOffset() => hitboxOffset;
    public Vector2 GetHitboxSize() => hitboxSize;
    public LayerMask GetHitLayer() => hitLayer;

    public float[] GetComboKnockback() => comboKnockback;
    public float GetAirAttackKnockback() => airAttackKnockback;
    public float[] GetComboHitStop() => comboHitStop;
    public float GetAirAttackHitStop() => airAttackHitStop;

    public float GetParryWindow() => parryWindow;
    public float GetDefendCooldown() => defendCooldown;
    public float GetBlockDamageMultiplier() => blockDamageMultiplier;
    public float GetParryHitStop() => parryHitStop;
    public int GetCounterStartFrame() => counterStartFrame;

    public int GetMaxHealth() => maxHealth;
    public LayerMask GetDamageLayers() => damageLayers;
    public float GetInvulnerableTime() => invulnerableTime;
    public float GetHurtDuration() => hurtDuration;
    public float GetDeathDuration() => deathDuration;
    public float GetRespawnDelay() => respawnDelay;

    public float GetKnockbackDuration() => knockbackDuration;
    public float GetKnockbackUpward() => knockbackUpward;
    public float GetBlockKnockbackMultiplier() => blockKnockbackMultiplier;

    public int GetHealCharges() => healCharges;
    public int GetHealAmount() => healAmount;
    public float GetHealDuration() => healDuration;

    public AudioClip GetJumpClip() => jumpClip;
    public AudioClip GetAirJumpClip() => airJumpClip;
    public AudioClip GetLandClip() => landClip;
    public AudioClip GetDashClip() => dashClip;
    public AudioClip[] GetAttackClips() => attackClips;
    public AudioClip GetAirAttackClip() => airAttackClip;
    public AudioClip GetHitClip() => hitClip;
    public AudioClip GetDefendClip() => defendClip;
    public AudioClip GetHurtClip() => hurtClip;
    public AudioClip GetHealClip() => healClip;
}
