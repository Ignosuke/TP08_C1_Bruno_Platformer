using UnityEngine;

// Al morir (PlayerHealth llama a StartRespawn) apaga los controles, espera la animacion de muerte y lleva al jugador al checkpoint tocado (o al inicio) con todo lleno
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private Checkpoint checkpoint;      // Lo pone el Checkpoint al tocarlo, hasta entonces vuelve al inicio

    private float respawnTime;
    private float fallGravity;

    private float respawnCounter;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private PlayerHealer healer;
    private Behaviour[] controls;

    private Vector3 startPosition;
    private bool isDead;

    private void Awake()
    {
        respawnTime = playerData.GetDeathDuration() + playerData.GetRespawnDelay();
        fallGravity = playerData.GetFallGravity();

        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        healer = GetComponent<PlayerHealer>();

        // A diferencia de PlayerKnockback tambien apaga la defensa y la curacion, muerto no puede hacer nada hasta reaparecer
        controls = new Behaviour[]
        {
            GetComponent<PlayerMover>(), GetComponent<PlayerJumper>(), GetComponent<PlayerDasher>(),
            GetComponent<PlayerAttacker>(), GetComponent<PlayerDefender>(), healer,
        };

        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isDead) return;

        respawnCounter -= Time.deltaTime;
        if (respawnCounter <= 0f) Respawn();
    }

    // Lo llama el Checkpoint cuando el jugador lo toca
    public void SetCheckpoint(Checkpoint value)
    {
        checkpoint = value;
    }

    // Lo llama PlayerHealth al morir
    public void StartRespawn()
    {
        isDead = true;
        respawnCounter = respawnTime;
        SetControls(false);

        // El material no tiene friccion, sin esto seguiria deslizando. Si muere en el dash la gravedad quedaba en 0
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.gravityScale = fallGravity;
    }

    private void Respawn()
    {
        isDead = false;

        Vector3 position = checkpoint != null ? checkpoint.GetRespawnPosition() : startPosition;
        rb.position = position;
        transform.position = position;
        rb.linearVelocity = Vector2.zero;

        SetControls(true);
        healer.RefillCharges();
        health.Revive();
    }

    private void SetControls(bool isEnabled)
    {
        foreach (Behaviour control in controls) control.enabled = isEnabled;
    }
}
