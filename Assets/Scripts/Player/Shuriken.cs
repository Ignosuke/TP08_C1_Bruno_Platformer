using System;
using UnityEngine;

// Shuriken del jugador. Viaja derecho girando y cada frame tira un rayo desde donde estaba hasta donde llega, asi no atraviesa nada.
// Al chocar con un enemigo le hace daño, y con un enemigo o el piso vuelve al pool. Tambien vuelve al agotar su vida
public class Shuriken : MonoBehaviour
{
    private Action<Shuriken> release;
    private Action onHit;
    private LayerMask hitMask;
    private Vector2 castOrigin;
    private int direction;
    private int damage;
    private float knockback;
    private float speed;
    private float spinSpeed;
    private float lifeCounter;

    // Lo lanza PlayerThrower. origin es el cuerpo del jugador, asi pega aunque el enemigo este pegado.
    // Se reusa, por eso arranca todo de cero
    public void Launch(PlayerSo playerData, int direction, Vector3 position, Vector2 origin, Action<Shuriken> release, Action onHit)
    {
        this.direction = direction;
        this.release = release;
        this.onHit = onHit;
        transform.position = position;
        transform.rotation = Quaternion.identity;
        castOrigin = origin;
        damage = playerData.GetShurikenDamage();
        knockback = playerData.GetShurikenKnockback();
        speed = playerData.GetShurikenSpeed();
        spinSpeed = playerData.GetShurikenSpinSpeed();
        lifeCounter = playerData.GetShurikenLifetime();
        hitMask = playerData.GetShurikenHitLayer();
    }

    private void Update()
    {
        lifeCounter -= Time.deltaTime;
        if (lifeCounter <= 0f)
        {
            release(this);
            return;
        }

        // Hacia la derecha gira en sentido horario
        transform.Rotate(0f, 0f, -direction * spinSpeed * Time.deltaTime);

        Vector2 forward = new Vector2(direction, 0f);
        Vector2 next = (Vector2)transform.position + forward * speed * Time.deltaTime;
        if (CheckHit(forward, Mathf.Abs(next.x - castOrigin.x))) return;

        transform.position = next;
        castOrigin = next;
    }

    // RaycastAll viene ordenado por distancia, gana lo primero solido. Los enemigos muertos no la frenan
    private bool CheckHit(Vector2 forward, float length)
    {
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(castOrigin, forward, length, hitMask))
        {
            if (hit.collider.isTrigger) continue;   // Zonas y checkpoints no la frenan

            if (hit.collider.TryGetComponent(out IDamageable target))
            {
                if (target is Health health && health.GetIsDead()) continue;
                target.TakeDamage(new DamageInfo(damage, forward, gameObject.layer, knockback));
            }

            onHit?.Invoke();
            release(this);
            return true;
        }
        return false;
    }
}
