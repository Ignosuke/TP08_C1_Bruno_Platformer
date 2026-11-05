using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Una fila de iconos llenos, a medias o vacios (corazones de vida, cargas de curacion).
// No sabe de quien es el valor, cualquiera le pasa el actual y el maximo con Refresh
public class IconRow : MonoBehaviour
{
    [SerializeField] private Sprite fullSprite;
    [Tooltip("Used when an icon holds some points but not all. Only needed if Points Per Icon is above 1")]
    [SerializeField] private Sprite halfSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private int pointsPerIcon = 1;     // Corazones 2, asi 1 punto es medio corazon
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Vector2 iconSize = new Vector2(16f, 16f);    // En pixeles del juego
    [SerializeField] private float spacing = 1f;

    [Header("Lose Feedback")]
    [Tooltip("Draws the icon as a flat silhouette of the flash color")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float shakeTime = 0.25f;
    [SerializeField] private float shakeStrength = 1.5f;

    [Header("Gain Feedback")]
    [SerializeField] private float popTime = 0.2f;
    [SerializeField] private float popScale = 1.4f;

    private Image[] icons = new Image[0];
    private Coroutine[] effects = new Coroutine[0];
    private int shownMax = -1;
    private int shown;      // Cuantos puntos se ven

    public void Refresh(int current, int max)
    {
        // Primera vez o cambio el maximo (mejora de vida), se arma de nuevo sin animar
        if (max != shownMax)
        {
            Build(max, current);
            return;
        }

        for (int i = 0; i < icons.Length; i++)
        {
            int before = GetIconPoints(i, shown);
            int after = GetIconPoints(i, current);
            if (after < before) Play(i, Lose(icons[i], GetSprite(after)));
            else if (after > before) Play(i, Gain(icons[i], GetSprite(after)));
        }

        shown = current;
    }

    // Cuantos de los puntos actuales caen en el icono i, entre 0 y pointsPerIcon
    private int GetIconPoints(int index, int points) => Mathf.Clamp(points - index * pointsPerIcon, 0, pointsPerIcon);

    private Sprite GetSprite(int iconPoints)
    {
        if (iconPoints <= 0) return emptySprite;
        if (iconPoints >= pointsPerIcon) return fullSprite;
        return halfSprite;
    }

    private void Build(int max, int current)
    {
        StopAllCoroutines();
        foreach (Image icon in icons) Destroy(icon.gameObject);

        int count = Mathf.CeilToInt((float)max / pointsPerIcon);
        icons = new Image[count];
        effects = new Coroutine[count];
        shownMax = max;
        shown = current;

        // El ancho de la fila es el de sus iconos, asi otra fila anclada a su centro queda centrada
        RectTransform rowRect = (RectTransform)transform;
        rowRect.sizeDelta = new Vector2(count * iconSize.x + Mathf.Max(0, count - 1) * spacing, iconSize.y);

        for (int i = 0; i < count; i++)
        {
            GameObject iconObject = new GameObject("Icon" + i, typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(transform, false);

            Image icon = iconObject.GetComponent<Image>();
            icon.sprite = GetSprite(GetIconPoints(i, current));
            icon.color = color;
            icon.raycastTarget = false;

            RectTransform rect = icon.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.sizeDelta = iconSize;
            rect.anchoredPosition = GetIconPosition(i);

            icons[i] = icon;
        }
    }

    private Vector2 GetIconPosition(int index) => new Vector2(index * (iconSize.x + spacing) + iconSize.x * 0.5f, 0f);

    // Si el icono ya estaba animando se corta, asi un efecto viejo no lo deja en el estado equivocado
    private void Play(int index, IEnumerator effect)
    {
        if (effects[index] != null) StopCoroutine(effects[index]);

        Image icon = icons[index];
        icon.material = null;
        icon.color = color;
        icon.rectTransform.localScale = Vector3.one;
        icon.rectTransform.anchoredPosition = GetIconPosition(index);

        effects[index] = StartCoroutine(effect);
    }

    // Destello blanco y temblor con el sprite que tenia, despues queda medio o vacio
    private IEnumerator Lose(Image icon, Sprite endSprite)
    {
        RectTransform rect = icon.rectTransform;
        Vector2 basePosition = rect.anchoredPosition;
        icon.material = flashMaterial;
        icon.color = flashColor;

        // Tiempo sin escala, asi el HUD sigue animando aunque el juego este en pausa
        float time = 0f;
        while (time < shakeTime)
        {
            time += Time.unscaledDeltaTime;

            if (time >= flashTime)
            {
                icon.material = null;
                icon.color = color;
            }

            // Pasos enteros para que el icono no se mueva entre pixeles
            rect.anchoredPosition = basePosition + new Vector2(Mathf.Round(Random.Range(-shakeStrength, shakeStrength)), 0f);
            yield return null;
        }

        rect.anchoredPosition = basePosition;
        icon.material = null;
        icon.color = color;
        icon.sprite = endSprite;
    }

    // Cambia al sprite nuevo y crece un poco, despues vuelve a su tamaño
    private IEnumerator Gain(Image icon, Sprite endSprite)
    {
        RectTransform rect = icon.rectTransform;
        icon.sprite = endSprite;

        float time = 0f;
        while (time < popTime)
        {
            time += Time.unscaledDeltaTime;
            float scale = Mathf.Lerp(popScale, 1f, time / popTime);
            rect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        rect.localScale = Vector3.one;
    }
}
