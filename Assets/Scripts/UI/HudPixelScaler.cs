using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

// Escala el canvas por un numero entero, el mismo zoom que usa la Pixel Perfect Camera.
// Asi un pixel del HUD mide igual que un pixel del juego y el pixel art no se deforma
[RequireComponent(typeof(CanvasScaler))]
public class HudPixelScaler : MonoBehaviour
{
    [SerializeField] private PixelPerfectCamera pixelCamera;

    private CanvasScaler scaler;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
    }

    // Cada frame por si cambia el tamano de la ventana
    private void Update()
    {
        int scale = Mathf.Min(Screen.width / pixelCamera.refResolutionX, Screen.height / pixelCamera.refResolutionY);
        scaler.scaleFactor = Mathf.Max(1, scale);
    }
}
