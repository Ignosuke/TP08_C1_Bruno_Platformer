using UnityEngine;

// Muestra el fondo de primavera o el de otoño segun la zona del area, con un fundido entre los dos.
// El otoño se dibuja siempre encima, asi el fundido es solo su transparencia y sirve para ir y para volver
public class ParallaxSwitcher : MonoBehaviour
{
    [SerializeField] private AreaTracker areaTracker;
    [SerializeField] private GameObject springSet;
    [SerializeField] private GameObject autumnSet;
    [SerializeField] private ParallaxSo springData;
    [SerializeField] private ParallaxSo autumnData;

    private SpriteRenderer[] autumnRenderers;
    private float springFadeTime;
    private float autumnFadeTime;

    private float autumnAlpha;      // 0 se ve solo primavera, 1 se ve solo otoño
    private float targetAlpha;
    private float fadeTime;
    private bool hasArea;           // La primera area se muestra sin fundido

    private void Awake()
    {
        // true para encontrarlos aunque el set de otoño arranque apagado
        autumnRenderers = autumnSet.GetComponentsInChildren<SpriteRenderer>(true);
        springFadeTime = springData.GetFadeTime();
        autumnFadeTime = autumnData.GetFadeTime();
    }

    private void OnEnable()
    {
        areaTracker.OnAreaChanged += ShowZone;
    }

    private void OnDisable()
    {
        areaTracker.OnAreaChanged -= ShowZone;
    }

    private void Update()
    {
        if (autumnAlpha == targetAlpha) return;

        autumnAlpha = Mathf.MoveTowards(autumnAlpha, targetAlpha, Time.deltaTime / fadeTime);
        ApplyAlpha();
    }

    // Sigue desde la transparencia actual, asi volver a mitad del fundido no hace saltos
    private void ShowZone(Area area)
    {
        bool isAutumn = area.GetAreaData().GetParallaxZone() == ParallaxZone.Autumn;
        targetAlpha = isAutumn ? 1f : 0f;
        fadeTime = isAutumn ? autumnFadeTime : springFadeTime;

        // Al arrancar o con tiempo 0 cambia en el acto
        if (!hasArea || fadeTime <= 0f)
        {
            autumnAlpha = targetAlpha;
            ApplyAlpha();
        }
        hasArea = true;
    }

    private void ApplyAlpha()
    {
        // Cada set se apaga recien cuando el otro lo tapa del todo, mientras tanto los dos se mueven
        springSet.SetActive(autumnAlpha < 1f);
        autumnSet.SetActive(autumnAlpha > 0f);

        foreach (SpriteRenderer autumnRenderer in autumnRenderers)
        {
            Color color = autumnRenderer.color;
            color.a = autumnAlpha;
            autumnRenderer.color = color;
        }
    }
}
