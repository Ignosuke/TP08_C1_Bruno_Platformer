using UnityEngine;

// La vida de un enemigo. Con cada golpe se desliza con un empujon que se va frenando, su fuerza viene del golpe.
// Al morir espera la animacion y se apaga, no se destruye porque vuelve cuando el jugador reaparece
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : Health
{
    [SerializeField] private EnemySo enemyData;

    private Rigidbody2D rb;
    private EnemyAttack attack;
    private float knockbackDirection;
    private float knockbackStrength;
    private float knockbackCounter;
    private float deathCounter;
    private bool isDying;

    public override int MaxHealth => enemyData.GetMaxHealth();
    protected override float InvulnerableTime => enemyData.GetInvulnerableTime();
    // Despues del punto sin retorno el golpe no lo hace quejarse, asi el ataque sigue
    protected override float HurtDuration => attack.GetIsArmored() ? 0f : enemyData.GetHurtDuration();
    protected override LayerMask DamageLayers => enemyData.GetDamageLayers();

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        attack = GetComponent<EnemyAttack>();
        OnDamaged += Push;
    }

    protected override void Update()
    {
        base.Update();

        if (isDying)
        {
            deathCounter -= Time.deltaTime;
            if (deathCounter <= 0f) gameObject.SetActive(false);
            return;
        }

        if (knockbackCounter <= 0f) return;

        knockbackCounter -= Time.deltaTime;
        float push = knockbackStrength * Mathf.Max(0f, knockbackCounter) / enemyData.GetHurtDuration();
        rb.linearVelocity = new Vector2(knockbackDirection * push, rb.linearVelocity.y);
    }

    // Lo empuja para donde viaja el golpe
    private void Push(DamageInfo info)
    {
        if (attack.GetIsArmored()) return;    // Con el ataque comprometido no lo mueven

        knockbackDirection = Mathf.Sign(info.Direction.x);
        knockbackStrength = info.Knockback;
        knockbackCounter = enemyData.GetHurtDuration();
    }

    protected override void Die()
    {
        isDying = true;
        knockbackCounter = 0f;
        deathCounter = enemyData.GetDeathDuration() + enemyData.GetCorpseTime();
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void ResetHealth()
    {
        isDying = false;
        knockbackCounter = 0f;
        Refill();
    }
}
