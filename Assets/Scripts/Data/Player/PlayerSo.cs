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

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float acceleration = 90f;      // Que tan rapido llega a la velocidad maxima
    [SerializeField] private float deceleration = 120f;     // Que tan rapido frena al soltar

    [Header("Salto")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float remainingForceOnTapJump = 0.45f; // Cuanto de la velocidad sobrevive al soltar
    [SerializeField] private float riseGravity = 3.5f;      // Mientras sube
    [SerializeField] private float fallGravity = 5.5f;      // Apenas empieza a caer
    [SerializeField] private float heavyFallGravity = 8f;   // Caida acelerada
    [SerializeField] private float heavyFallThreshold = 12f; // Velocidad de caida a la que se activa
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpLockTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Suelo")]
    [SerializeField] private float feetWidth = 0.3f;        // Ancho del personaje a la altura de los pies
    [SerializeField] private float checkDistance = 0.2f;    // Distancia del raycast para detectar el suelo
    [SerializeField] private LayerMask groundLayer;         // Solo la capa Ground: los rayos salen de los pies y tocarian al propio jugador

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 24f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;     // Se cuenta desde que arranca el dash

    [Header("Ataque")]
    [SerializeField] private int[] comboDamage = { 1, 1, 2 };            // Ataque 1, 2 y 3
    [SerializeField] private float[] comboDuration = { 0.25f, 0.25f, 0.4f };
    [SerializeField] private int airAttackDamage = 1;
    [SerializeField] private float airAttackDuration = 0.3f;
    [SerializeField] private float attackHitDelay = 0.08f;  // Tiempo desde que arranca el golpe hasta que pega
    [SerializeField] private float comboWindow = 0.3f;      // Tiempo despues de un golpe para encadenar el siguiente
    [SerializeField] private float attackBufferTime = 0.2f;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1.1f, 0f); // Mirando a la derecha
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.6f, 1.3f);
    [SerializeField] private LayerMask hitLayer = ~0;

    [Header("Defensa")]
    [SerializeField] private float parryWindow = 0.15f;     // Primeros segundos de la defensa que cuentan como parry
    [SerializeField] private float defendCooldown = 0.25f;  // Para que no se pueda spamear el parry

    [Header("Sonidos")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip[] attackClips = new AudioClip[3];
    [SerializeField] private AudioClip airAttackClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip defendClip;

    public InputActionReference GetMoveAction() => moveAction;
    public InputActionReference GetJumpAction() => jumpAction;
    public InputActionReference GetAttackAction() => attackAction;
    public InputActionReference GetDashAction() => dashAction;
    public InputActionReference GetDefendAction() => defendAction;

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
    public float GetAttackHitDelay() => attackHitDelay;
    public float GetComboWindow() => comboWindow;
    public float GetAttackBufferTime() => attackBufferTime;
    public Vector2 GetHitboxOffset() => hitboxOffset;
    public Vector2 GetHitboxSize() => hitboxSize;
    public LayerMask GetHitLayer() => hitLayer;

    public float GetParryWindow() => parryWindow;
    public float GetDefendCooldown() => defendCooldown;

    public AudioClip GetJumpClip() => jumpClip;
    public AudioClip GetLandClip() => landClip;
    public AudioClip GetDashClip() => dashClip;
    public AudioClip[] GetAttackClips() => attackClips;
    public AudioClip GetAirAttackClip() => airAttackClip;
    public AudioClip GetHitClip() => hitClip;
    public AudioClip GetDefendClip() => defendClip;
}
