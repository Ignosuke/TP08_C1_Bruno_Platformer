using UnityEngine;

// Muñeco de prueba, parpadea y loguea al recibir un golpe, nunca muere
[RequireComponent(typeof(SpriteRenderer))]
// Suggestion: este componente no esta en ninguna escena ni prefab (codigo muerto de prueba). Borralo o movelo a una carpeta Test/.
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

    public bool TakeDamage(DamageInfo info)
    {
        totalDamage += info.Amount;
        Debug.Log(name + " took " + info.Amount + " damage (total " + totalDamage + ")");

        spriteRenderer.color = flashColor;
        flashCounter = flashTime;
        return false;
    }
}
