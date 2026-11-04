using UnityEngine;

public static class LayerMaskExtensions
{
    // True si la capa esta prendida en la mascara
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
