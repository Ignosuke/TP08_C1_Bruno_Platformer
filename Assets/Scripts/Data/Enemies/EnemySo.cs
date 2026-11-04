using UnityEngine;

// Los valores de un enemigo. Cada enemigo tiene su asset, el Oni y el Executioner cambian solo aca
[CreateAssetMenu(fileName = "EnemySo", menuName = "Data/Enemy")]
public class EnemySo : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 8;                 // El combo del jugador saca 4
    [SerializeField] private float invulnerableTime = 0.15f;    // Asi un mismo golpe no cuenta dos veces
    [SerializeField] private float hurtDuration = 0.4f;         // Lo que dura la animacion Hurt
    [SerializeField] private float deathDuration = 1.25f;       // Lo que dura la animacion Death
    [SerializeField] private float corpseTime = 0.4f;           // Tiempo en el piso antes de desaparecer
    [SerializeField] private LayerMask damageLayers;            // Capas que lo lastiman al tocarlas, por ahora ninguna

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;            // Patrullando
    [SerializeField] private float chaseSpeed = 2.5f;           // Persiguiendo al jugador
    [SerializeField] private float edgeCheckDistance = 0.6f;    // Cuanto debajo de frontCheck busca piso
    [SerializeField] private float wallCheckDistance = 0.2f;    // Cuanto adelante de frontCheck busca pared
    [SerializeField] private float turnPauseDuration = 0.4f;    // Quieto antes de girar en una pared o un borde
    [SerializeField] private LayerMask groundLayer;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float verticalTolerance = 1.2f;    // El pivot del jugador esta en su centro, medio metro arriba de sus pies

    [Header("Attack")]
    [SerializeField] private int damage = 2;                    // En medios corazones
    [SerializeField] private float attackRange = 1.8f;          // Distancia a la que empieza a atacar
    [Tooltip("Sheet frame (1-based) where the attack lands. The builder turns it into an Animation Event")]
    [SerializeField] private int attackHitFrame = 10;
    [Tooltip("Sheet frame (1-based) from which player hits no longer interrupt the attack. 0 = never armored")]
    [SerializeField] private int attackCommitFrame = 6;
    [SerializeField] private float attackDuration = 1.2f;       // Lo que dura la animacion Attack
    [SerializeField] private float attackCooldown = 1f;         // Se cuenta desde que termina el ataque
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1.4f, 0.5f);   // Mirando a la derecha, desde los pies
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.8f, 1f);
    [SerializeField] private LayerMask targetLayer;             // Donde esta el jugador
    [SerializeField] private float stunDuration = 1.5f;         // Quieto en Hurt despues de un parry perfecto
    [Tooltip("Push given to the player by this attack")]
    [SerializeField] private float attackKnockback = 8f;
    [Tooltip("Game freeze in real seconds when the attack lands, 0 = none")]
    [SerializeField] private float attackHitStop = 0.08f;
    [Tooltip("Camera shake strength in world units when the attack lands, 0 = none")]
    [SerializeField] private float attackShakeMagnitude = 0.15f;
    [SerializeField] private float attackShakeDuration = 0.25f;

    [Header("Contact")]
    [Tooltip("Damage when the player touches the body. Only used if the prefab has EnemyContactDamage")]
    [SerializeField] private int contactDamage = 1;
    [Tooltip("Push given to the player when touching the body")]
    [SerializeField] private float contactKnockback = 5f;

    [Header("Visual")]
    [Tooltip("True when the sprite sheets are drawn looking left")]
    [SerializeField] private bool spriteFacesLeft;

    [Header("Sounds")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    public int GetMaxHealth() => maxHealth;
    public float GetInvulnerableTime() => invulnerableTime;
    public float GetHurtDuration() => hurtDuration;
    public float GetDeathDuration() => deathDuration;
    public float GetCorpseTime() => corpseTime;
    public LayerMask GetDamageLayers() => damageLayers;

    public float GetMoveSpeed() => moveSpeed;
    public float GetChaseSpeed() => chaseSpeed;
    public float GetEdgeCheckDistance() => edgeCheckDistance;
    public float GetWallCheckDistance() => wallCheckDistance;
    public float GetTurnPauseDuration() => turnPauseDuration;
    public LayerMask GetGroundLayer() => groundLayer;

    public float GetDetectionRange() => detectionRange;
    public float GetVerticalTolerance() => verticalTolerance;

    public int GetDamage() => damage;
    public float GetAttackRange() => attackRange;
    public int GetAttackHitFrame() => attackHitFrame;
    public int GetAttackCommitFrame() => attackCommitFrame;
    public float GetAttackDuration() => attackDuration;
    public float GetAttackCooldown() => attackCooldown;
    public Vector2 GetHitboxOffset() => hitboxOffset;
    public Vector2 GetHitboxSize() => hitboxSize;
    public LayerMask GetTargetLayer() => targetLayer;
    public float GetStunDuration() => stunDuration;
    public float GetAttackKnockback() => attackKnockback;
    public float GetAttackHitStop() => attackHitStop;
    public float GetAttackShakeMagnitude() => attackShakeMagnitude;
    public float GetAttackShakeDuration() => attackShakeDuration;

    public int GetContactDamage() => contactDamage;
    public float GetContactKnockback() => contactKnockback;

    public bool GetSpriteFacesLeft() => spriteFacesLeft;

    public AudioClip GetAttackClip() => attackClip;
    public AudioClip GetHurtClip() => hurtClip;
    public AudioClip GetDeathClip() => deathClip;
}
