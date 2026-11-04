using UnityEngine;

// Cuanto sigue cada capa de un fondo a la camara, de atras hacia adelante
[CreateAssetMenu(fileName = "ParallaxSo", menuName = "Data/Parallax")]
public class ParallaxSo : ScriptableObject
{
    [Tooltip("Back to front. 0 = stays with the world, 1 = glued to the camera")]
    [SerializeField] private float[] layerFactors;
    [Tooltip("Same order as Layer Factors. Units per second the layer moves on its own, negative = left. Missing entries = 0")]
    [SerializeField] private float[] layerDrift = new float[0];
    [Tooltip("Seconds this background takes to appear when its zone is entered. 0 = instant cut")]
    [SerializeField] private float fadeTime = 1f;

    public float GetLayerFactor(int index) => layerFactors[index];
    // Si el array es mas corto (el otono no tiene) la capa no se mueve sola
    public float GetLayerDrift(int index) => index < layerDrift.Length ? layerDrift[index] : 0f;
    public int GetLayerCount() => layerFactors.Length;
    public float GetFadeTime() => fadeTime;
}
