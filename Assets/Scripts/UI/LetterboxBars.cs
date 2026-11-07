using UnityEngine;

// Dos franjas negras arriba y abajo para las escenas. Crecen desde los bordes al entrar y se achican al salir
public class LetterboxBars : MonoBehaviour
{
    [Tooltip("Stretched across the top edge, pivot on top")]
    [SerializeField] private RectTransform top;
    [Tooltip("Stretched across the bottom edge, pivot on the bottom")]
    [SerializeField] private RectTransform bottom;
    [SerializeField] private float height = 32f;        // En pixeles del juego
    [SerializeField] private float slideTime = 0.35f;

    private float shown;        // 0 afuera, 1 adentro
    private float target;

    private void OnEnable()
    {
        shown = 0f;
        target = 0f;
        Apply();
    }

    public void Show() => target = 1f;
    public void Hide() => target = 0f;

    // Tiempo sin escala, asi el hitstop no las frena
    private void Update()
    {
        if (shown == target) return;

        shown = Mathf.MoveTowards(shown, target, slideTime > 0f ? Time.unscaledDeltaTime / slideTime : 1f);
        Apply();
    }

    // Alto entero para que el borde no quede entre pixeles
    private void Apply()
    {
        float barHeight = Mathf.Round(height * shown);
        top.sizeDelta = new Vector2(0f, barHeight);
        bottom.sizeDelta = new Vector2(0f, barHeight);
    }
}
