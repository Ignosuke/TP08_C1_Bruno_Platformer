using System;
using UnityEngine;

// Flecha del arquero. Viaja derecho y cada frame tira un rayo desde donde estaba hasta donde llega, asi no atraviesa nada.
// Choca con el jugador o el piso, muestra el impacto un momento y vuelve al pool. Si choca con algo que recibe daño lo sigue mientras dura el impacto.
// Con parry perfecto desaparece, suelta sus astillas y vuelve al pool cuando terminan.
// Los enemigos no estan en la mascara
public class Arrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] hitFrames;
    [SerializeField] private float hitFramesPerSecond = 20f;
    [SerializeField] private ParticleSystem parryBurst;
    [Tooltip("How far the arrow sinks into a damageable target on hit, in units")]
    [SerializeField] private float hitInset = 0.125f;

    private Sprite flightSprite;
    private Action<Arrow> release;
    private EnemyAttackData attack;
    private LayerMask hitMask;
    private Vector2 castOrigin;
    private int direction;
    private float speed;
    private float lifeCounter;
    private float hitCounter;
    private bool hasHit;
    private bool wasParried;
    private Transform hitTarget;
    private Vector3 hitOffset;

    private void Awake()
    {
        flightSprite = spriteRenderer.sprite;
    }

    // La dispara RangedAttack. origin es el cuerpo del que dispara, asi pega aunque el jugador este pegado.
    // Se reusa, por eso arranca todo de cero
    public void Launch(EnemySo enemyData, EnemyAttackData attack, int direction, Vector3 position, Vector2 origin, Action<Arrow> release)
    {
        this.attack = attack;
        this.direction = direction;
        this.release = release;
        transform.position = position;
        castOrigin = origin;
        speed = enemyData.GetArrowSpeed();
        lifeCounter = enemyData.GetArrowLifetime();
        hitMask = enemyData.GetTargetLayer() | enemyData.GetGroundLayer();
        hitCounter = 0f;
        hasHit = false;
        wasParried = false;
        hitTarget = null;
        parryBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = flightSprite;
        spriteRenderer.flipX = direction < 0;   // Las hojas miran a la derecha
    }

    private void Update()
    {
        if (wasParried)
        {
            if (!parryBurst.IsAlive(true)) release(this);
            return;
        }

        if (hasHit)
        {
            PlayHit();
            return;
        }

        lifeCounter -= Time.deltaTime;
        if (lifeCounter <= 0f)
        {
            release(this);
            return;
        }

        Vector2 forward = new Vector2(direction, 0f);
        Vector2 next = (Vector2)transform.position + forward * speed * Time.deltaTime;
        if (CheckHit(forward, Mathf.Abs(next.x - castOrigin.x))) return;

        transform.position = next;
        castOrigin = next;
    }

    // RaycastAll viene ordenado por distancia, gana lo primero solido
    private bool CheckHit(Vector2 forward, float length)
    {
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(castOrigin, forward, length, hitMask))
        {
            if (hit.collider.isTrigger) continue;   // Zonas y checkpoints no la frenan

            transform.position = new Vector3(hit.point.x, transform.position.y, transform.position.z);
            bool isDamageable = hit.collider.TryGetComponent(out IDamageable target);

            // El parry perfecto la saca en el acto, no aturde a nadie
            if (isDamageable && target.TakeDamage(new DamageInfo(attack.GetDamage(), forward, gameObject.layer, attack.GetKnockback())))
            {
                PlayParry();
                return true;
            }

            // Al que recibe daño lo sigue, el piso y las paredes no se mueven
            // Se clava unos pixeles, el collider es mas ancho que el dibujo
            if (isDamageable)
            {
                transform.position += new Vector3(direction * hitInset, 0f, 0f);
                hitTarget = hit.collider.transform;
                hitOffset = transform.position - hitTarget.position;
            }
            spriteRenderer.sprite = hitFrames[0];
            hasHit = true;
            return true;
        }
        return false;
    }

    // Se esconde y queda quieta hasta que terminan las astillas, asi el pool no la reusa antes
    private void PlayParry()
    {
        wasParried = true;
        spriteRenderer.enabled = false;
        parryBurst.Play(true);
    }

    // Los cuadros del impacto una sola vez y vuelve al pool
    private void PlayHit()
    {
        hitCounter += Time.deltaTime;
        int frame = (int)(hitCounter * hitFramesPerSecond);
        if (frame >= hitFrames.Length)
        {
            release(this);
            return;
        }
        FollowTarget();
        spriteRenderer.sprite = hitFrames[frame];
    }

    // Si el objetivo ya no esta termina donde quedo
    private void FollowTarget()
    {
        if (hitTarget == null) return;
        if (!hitTarget.gameObject.activeInHierarchy)
        {
            hitTarget = null;
            return;
        }
        transform.position = hitTarget.position + hitOffset;
    }
}
