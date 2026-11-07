using UnityEngine;

// Destello blanco en cada golpe que saca vida, aunque el enemigo tenga armadura y no pase por Hurt.
// Cambia el material del sprite por uno que lo dibuja como silueta del color del sprite, blanco
public class EnemyHitFlash : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Draws the sprite as a flat silhouette of its color")]
    [SerializeField] private Material flashMaterial;
    [Tooltip("Scaled seconds, so the flash holds through the hitstop")]
    [SerializeField] private float flashTime = 0.08f;

    private Material baseMaterial;
    private float flashCounter;

    private void Awake()
    {
        baseMaterial = spriteRenderer.sharedMaterial;
    }

    private void OnEnable()
    {
        health.OnDamaged += Flash;
    }

    private void OnDisable()
    {
        health.OnDamaged -= Flash;
        flashCounter = 0f;
        spriteRenderer.sharedMaterial = baseMaterial;
    }

    private void Update()
    {
        if (flashCounter <= 0f) return;

        flashCounter -= Time.deltaTime;
        if (flashCounter <= 0f) spriteRenderer.sharedMaterial = baseMaterial;
    }

    private void Flash(DamageInfo info)
    {
        spriteRenderer.sharedMaterial = flashMaterial;
        flashCounter = flashTime;
    }
}
