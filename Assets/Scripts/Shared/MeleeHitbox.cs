using System;
using UnityEngine;

// Golpe cuerpo a cuerpo con una caja adelante, lo usan el jugador y los enemigos.
// La caja se espeja segun hacia donde mira y el propio cuerpo se saltea
[RequireComponent(typeof(Rigidbody2D))]
public class MeleeHitbox : MonoBehaviour
{
    private const float GizmoTime = 0.2f;   // Cuanto se ve la ultima caja en la escena

    [SerializeField] private HitStopChannel hitStopChannel;
    [SerializeField] private CameraShakeChannel cameraShakeChannel;

    private Rigidbody2D rb;
    private Vector2 lastCenter;
    private Vector2 lastSize;
    private float gizmoCounter;

    public event Action OnHit;              // Una vez por cada cosa golpeada

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (gizmoCounter > 0f) gizmoCounter -= Time.deltaTime;
    }

    // offset es mirando a la derecha y facing es 1 o -1. Devuelve true si algun objetivo hizo parry perfecto.
    // El hitstop y el temblor salen una sola vez y solo si golpeo algo. Sin temblor no se pasan sus valores
    public bool Hit(int damage, float knockback, Vector2 offset, Vector2 size, LayerMask mask, int facing,
        float hitStop, float shakeMagnitude = 0f, float shakeDuration = 0f)
    {
        lastCenter = (Vector2)transform.position + new Vector2(offset.x * facing, offset.y);
        lastSize = size;
        gizmoCounter = GizmoTime;

        DamageInfo info = new DamageInfo(damage, new Vector2(facing, 0f), gameObject.layer, knockback);
        bool parried = false;
        bool landed = false;

        foreach (Collider2D hit in Physics2D.OverlapBoxAll(lastCenter, size, 0f, mask))
        {
            if (hit.attachedRigidbody == rb) continue;   // No pegarse a uno mismo

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            if (damageable.TakeDamage(info)) parried = true;
            landed = true;
            OnHit?.Invoke();
        }

        if (landed)
        {
            hitStopChannel.Raise(hitStop);
            cameraShakeChannel.Raise(shakeMagnitude, shakeDuration);
        }
        return parried;
    }

    private void OnDrawGizmos()
    {
        if (gizmoCounter <= 0f) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(lastCenter, lastSize);
    }
}
