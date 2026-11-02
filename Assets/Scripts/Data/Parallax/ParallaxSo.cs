using UnityEngine;

// Cuanto sigue cada capa de un fondo a la camara, de atras hacia adelante
[CreateAssetMenu(fileName = "ParallaxSo", menuName = "Data/Parallax")]
public class ParallaxSo : ScriptableObject
{
    [Tooltip("Back to front. 0 = stays with the world, 1 = glued to the camera")]
    [SerializeField] private float[] layerFactors;
    [Tooltip("Seconds this background takes to appear when its zone is entered. 0 = instant cut")]
    [SerializeField] private float fadeTime = 1f;

    public float GetLayerFactor(int index) => layerFactors[index];
    public int GetLayerCount() => layerFactors.Length;
    public float GetFadeTime() => fadeTime;
}
