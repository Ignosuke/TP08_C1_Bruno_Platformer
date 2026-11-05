using UnityEngine;

// Empuja al enemigo cuando recibe un golpe, para donde viaja el golpe y con la fuerza que trae el DamageInfo.
// El empujon se va frenando mientras dura el Hurt. Despues del punto sin retorno no lo mueven
[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private EnemySo enemyData;

    private float direction;
    private float strength;
    private float duration;
    private float knockbackCounter;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyAttack attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        attack = GetComponent<EnemyAttack>();
        if (attack == null) Debug.LogError(name + " needs an EnemyAttack component", this);
    }

    private void OnEnable()
    {
        health.OnDamaged += Push;
        health.OnDied += Stop;
    }

    private void OnDisable()
    {
        health.OnDamaged -= Push;
        health.OnDied -= Stop;
        knockbackCounter = 0f;
    }

    private void Update()
    {
        if (knockbackCounter <= 0f) return;

        knockbackCounter -= Time.deltaTime;
        float push = strength * Mathf.Max(0f, knockbackCounter) / duration;
        rb.linearVelocity = new Vector2(direction * push, rb.linearVelocity.y);
    }

    private void Push(DamageInfo info)
    {
        if (attack.GetIsArmored()) return;

        // Sin Hurt no hay empujon, y asi nunca divide por cero
        duration = enemyData.GetHurtDuration();
        if (duration <= 0f) return;

        direction = Mathf.Sign(info.Direction.x);
        strength = info.Knockback;
        knockbackCounter = duration;
    }

    // Muerto no se desliza
    private void Stop()
    {
        knockbackCounter = 0f;
    }
}
