using UnityEngine;

// Muñeco de prueba, parpadea y loguea al recibir un golpe, nunca muere
[RequireComponent(typeof(SpriteRenderer))]
public class TrainingDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.12f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float flashCounter;
    private int totalDamage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (flashCounter <= 0f) return;

        flashCounter -= Time.deltaTime;
        if (flashCounter <= 0f) spriteRenderer.color = baseColor;
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        totalDamage += amount;
        Debug.Log(name + " recibio " + amount + " de danio (total " + totalDamage + ")");

        spriteRenderer.color = flashColor;
        flashCounter = flashTime;
    }
}
