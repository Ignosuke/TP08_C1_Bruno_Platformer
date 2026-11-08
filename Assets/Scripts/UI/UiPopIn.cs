using System.Collections;
using UnityEngine;

// Aparece creciendo hasta un maximo y vuelve a su tamaño. En tiempo real, asi anda aunque el juego este frenado
public class UiPopIn : MonoBehaviour
{
    [SerializeField] private float maxScale = 1.3f;
    [SerializeField] private float growTime = 0.12f;      // Segundos reales
    [SerializeField] private float settleTime = 0.1f;

    // Escala 0 y no apagado, asi el layout le guarda el lugar y la caja no cambia de tamaño
    public void Hide()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.zero;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(Pop());
    }

    private IEnumerator Pop()
    {
        yield return ScaleTo(0f, maxScale, growTime);
        yield return ScaleTo(maxScale, 1f, settleTime);
    }

    private IEnumerator ScaleTo(float from, float to, float duration)
    {
        for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(from, to, time / duration);
            yield return null;
        }
        transform.localScale = Vector3.one * to;
    }
}
