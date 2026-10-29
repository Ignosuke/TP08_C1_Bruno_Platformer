using UnityEngine;

// Como se ve cada area, el otoño se va oscureciendo hacia el jefe
[CreateAssetMenu(fileName = "AreaSo", menuName = "Data/Area")]
public class AreaSo : ScriptableObject
{
    [SerializeField] private Color lightColor = Color.white;    // Tinte de la luz global
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float lightFadeTime = 1f;          // Segundos para pasar al tinte de esta area

    public Color GetLightColor() => lightColor;
    public float GetLightIntensity() => lightIntensity;
    public float GetLightFadeTime() => lightFadeTime;
}
