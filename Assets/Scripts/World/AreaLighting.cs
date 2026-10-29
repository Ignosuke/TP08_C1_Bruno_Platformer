using UnityEngine;
using UnityEngine.Rendering.Universal;

// Al cambiar de area pasa la luz global al tinte de esa area de a poco
[RequireComponent(typeof(Light2D))]
public class AreaLighting : MonoBehaviour
{
    [SerializeField] private AreaTracker areaTracker;

    private Light2D globalLight;

    private Color fromColor;
    private Color toColor;
    private float fromIntensity;
    private float toIntensity;
    private float fadeTime;
    private float fadeCounter;

    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
        toColor = globalLight.color;
        toIntensity = globalLight.intensity;
    }

    private void OnEnable()
    {
        areaTracker.OnAreaChanged += StartFade;
    }

    private void OnDisable()
    {
        areaTracker.OnAreaChanged -= StartFade;
    }

    private void Update()
    {
        if (fadeCounter >= fadeTime) return;

        fadeCounter += Time.deltaTime;
        float t = Mathf.Clamp01(fadeCounter / fadeTime);
        globalLight.color = Color.Lerp(fromColor, toColor, t);
        globalLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
    }

    // Arranca desde el color actual, asi cruzar rapido varias areas no hace saltos
    private void StartFade(Area area)
    {
        AreaSo areaData = area.GetAreaData();

        fromColor = globalLight.color;
        fromIntensity = globalLight.intensity;
        toColor = areaData.GetLightColor();
        toIntensity = areaData.GetLightIntensity();
        fadeTime = areaData.GetLightFadeTime();
        fadeCounter = 0f;

        // Con tiempo 0 cambia en el acto
        if (fadeTime <= 0f)
        {
            globalLight.color = toColor;
            globalLight.intensity = toIntensity;
        }
    }
}
