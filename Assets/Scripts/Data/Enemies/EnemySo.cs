using System.Collections.Generic;
using UnityEngine;

// Los valores de un enemigo. Cada enemigo tiene su asset, el Oni, el Executioner y el Archer cambian solo aca
[CreateAssetMenu(fileName = "EnemySo", menuName = "Data/Enemy")]
public class EnemySo : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 8;                 // El combo del jugador saca 4
    [SerializeField] private float invulnerableTime = 0.15f;    // Asi un mismo golpe no cuenta dos veces
    [SerializeField] private float hurtDuration = 0.4f;         // Lo que dura la animacion Hurt
    [SerializeField] private float deathDuration = 1.25f;       // Lo que dura la animacion Death
    [SerializeField] private float corpseTime = 0.4f;           // Tiempo en el piso antes de desaparecer

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
    [SerializeField] private float attackRange = 1.8f;          // Distancia a la que empieza a atacar
    [SerializeField] private float attackCooldown = 1f;         // Se cuenta desde que termina el ataque
    [SerializeField] private LayerMask targetLayer;             // Donde esta el jugador
    [SerializeField] private float stunDuration = 1.5f;         // Quieto en Hurt despues de un parry perfecto
    [Tooltip("Seconds (scaled time) the parried attack holds its impact frame before the stun starts")]
    [SerializeField] private float parryRecoilTime = 0.05f;
    [Tooltip("Attacks used in turns, in this order. At least one. The builder fills it only when it is empty")]
    [SerializeField] private List<EnemyAttackData> attacks = new List<EnemyAttackData>();

    [Header("Ranged")]
    [Tooltip("Projectile shot by RangedAttack. Only used by ranged enemies")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float arrowLifetime = 1.5f;        // Si no choca con nada se destruye sola
    [Tooltip("Where the arrow tip spawns, looking right, from the feet")]
    [SerializeField] private Vector2 arrowSpawnOffset = new Vector2(0.9f, 0.75f);

    [Header("Keep Distance")]
    [Tooltip("Distance to the player it tries to keep while shooting")]
    [SerializeField] private float preferredDistance = 5.5f;
    [Tooltip("If the player gets closer than this it dashes away")]
    [SerializeField] private float fleeDistance = 2.5f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 2.5f;         // Se cuenta desde que arranca el dash

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

    public float GetMoveSpeed() => moveSpeed;
    public float GetChaseSpeed() => chaseSpeed;
    public float GetEdgeCheckDistance() => edgeCheckDistance;
    public float GetWallCheckDistance() => wallCheckDistance;
    public float GetTurnPauseDuration() => turnPauseDuration;
    public LayerMask GetGroundLayer() => groundLayer;

    public float GetDetectionRange() => detectionRange;
    public float GetVerticalTolerance() => verticalTolerance;

    public float GetAttackRange() => attackRange;
    public float GetAttackCooldown() => attackCooldown;
    public LayerMask GetTargetLayer() => targetLayer;
    public float GetStunDuration() => stunDuration;
    public float GetParryRecoilTime() => parryRecoilTime;
    public int GetAttackCount() => attacks.Count;
    public EnemyAttackData GetAttack(int index) => attacks[index];

    public Arrow GetArrowPrefab() => arrowPrefab;
    public float GetArrowSpeed() => arrowSpeed;
    public float GetArrowLifetime() => arrowLifetime;
    public Vector2 GetArrowSpawnOffset() => arrowSpawnOffset;

    public float GetPreferredDistance() => preferredDistance;
    public float GetFleeDistance() => fleeDistance;
    public float GetDashSpeed() => dashSpeed;
    public float GetDashDuration() => dashDuration;
    public float GetDashCooldown() => dashCooldown;

    public int GetContactDamage() => contactDamage;
    public float GetContactKnockback() => contactKnockback;

    public bool GetSpriteFacesLeft() => spriteFacesLeft;

    public AudioClip GetAttackClip() => attackClip;
    public AudioClip GetHurtClip() => hurtClip;
    public AudioClip GetDeathClip() => deathClip;
}
