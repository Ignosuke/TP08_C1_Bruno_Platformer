using UnityEngine;

// Empuja al jugador cuando recibe un golpe, hacia donde viaja el golpe y un poco para arriba,
// con la fuerza que trae el DamageInfo. Mientras dura apaga el movimiento, si no lo pisaria.
// El parry perfecto no llega aca porque no dispara OnDamaged, el bloqueo empuja menos
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private float duration;
    private float upward;
    private float blockMultiplier;
    private float fallGravity;

    private float knockbackCounter;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private Behaviour[] controls;

    private void Awake()
    {
        duration = playerData.GetKnockbackDuration();
        upward = playerData.GetKnockbackUpward();
        blockMultiplier = playerData.GetBlockKnockbackMultiplier();
        fallGravity = playerData.GetFallGravity();

        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();

        // Sin defensa ni curacion, apagar la defensa cortaria el bloqueo que provoco este empujon y la curacion ya se corta sola con el golpe
        controls = new Behaviour[]
        {
            GetComponent<PlayerMover>(), GetComponent<PlayerJumper>(), GetComponent<PlayerDasher>(), GetComponent<PlayerAttacker>(),
        };
    }

    private void OnEnable()
    {
        health.OnDamaged += Push;
    }

    private void OnDisable()
    {
        health.OnDamaged -= Push;
        if (knockbackCounter > 0f) RestoreControl();
        knockbackCounter = 0f;
    }

    private void Update()
    {
        if (knockbackCounter <= 0f) return;

        knockbackCounter -= Time.deltaTime;
        if (knockbackCounter <= 0f) RestoreControl();
    }

    private void Push(DamageInfo info)
    {
        float strength = info.Knockback;
        if (health.GetIsBlocking(info)) strength *= blockMultiplier;
        if (strength <= 0f || info.Direction.x == 0f) return;

        SetControls(false);
        knockbackCounter = duration;

        // Si lo golpean en el dash la gravedad quedaba en 0, igual que en PlayerRespawner
        rb.gravityScale = fallGravity;
        rb.linearVelocity = new Vector2(Mathf.Sign(info.Direction.x) * strength, strength * upward);
    }

    // Nunca devuelve el control si murio, de eso se encarga PlayerRespawner
    private void RestoreControl()
    {
        if (health.GetIsDead()) return;
        SetControls(true);
    }

    private void SetControls(bool isEnabled)
    {
        foreach (Behaviour control in controls) control.enabled = isEnabled;
    }
}
