using UnityEngine;

// Barrita de vida debajo de los pies, como la del TP anterior pero con sprites.
// El container se esconde con la vida llena o en cero, asi aparece con el primer golpe
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [Tooltip("Fill sprite with its pivot on the left. It shrinks on X")]
    [SerializeField] private Transform fill;
    [SerializeField] private GameObject container;

    private void OnEnable()
    {
        health.OnHealthChanged += UpdateBar;
        container.SetActive(false);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        fill.localScale = new Vector3(max > 0 ? (float)current / max : 0f, 1f, 1f);
        container.SetActive(current > 0 && current < max);
    }
}
