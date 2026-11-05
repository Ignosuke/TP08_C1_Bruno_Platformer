using UnityEngine;

// La vida de un enemigo. Al morir espera la animacion y se apaga, no se destruye porque vuelve cuando el jugador reaparece.
// El empujon de cada golpe lo hace EnemyKnockback
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : Health
{
    [SerializeField] private EnemySo enemyData;

    private Rigidbody2D rb;
    private EnemyAttack attack;
    private float deathCounter;
    private bool isDying;

    public override int MaxHealth => enemyData.GetMaxHealth();
    protected override float InvulnerableTime => enemyData.GetInvulnerableTime();
    // Despues del punto sin retorno el golpe no lo hace quejarse, asi el ataque sigue
    protected override float HurtDuration => attack.GetIsArmored() ? 0f : enemyData.GetHurtDuration();
    // Los enemigos no reciben golpes de contacto, solo ataques
    protected override LayerMask DamageLayers => 0;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        attack = GetComponent<EnemyAttack>();
        if (attack == null) Debug.LogError(name + " needs an EnemyAttack component", this);
    }

    protected override void Update()
    {
        base.Update();
        if (!isDying) return;

        deathCounter -= Time.deltaTime;
        if (deathCounter <= 0f) gameObject.SetActive(false);
    }

    protected override void Die()
    {
        isDying = true;
        deathCounter = enemyData.GetDeathDuration() + enemyData.GetCorpseTime();
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void ResetHealth()
    {
        isDying = false;
        Refill();
    }
}
