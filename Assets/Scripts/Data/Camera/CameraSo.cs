using UnityEngine;

// Valores de la camara que sigue al jugador
[CreateAssetMenu(fileName = "CameraSo", menuName = "Data/Camera")]
public class CameraSo : ScriptableObject
{
    [SerializeField] private float deadZoneWidth = 1.5f;    // Rectangulo donde el jugador se mueve sin mover la camara
    [SerializeField] private float deadZoneHeight = 2f;
    [SerializeField] private float smoothTime = 0.15f;      // Cuanto tarda en alcanzar al jugador
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);  // Hacia arriba para ver mas de lo que viene que del piso

    [Tooltip("When focusing something else, how far from the screen center it may stay when the area bounds stop the camera, as a fraction of half the screen width. 0.33 keeps it inside the middle third")]
    [SerializeField] private float focusMaxOffset = 0.33f;

    public float GetDeadZoneWidth() => deadZoneWidth;
    public float GetFocusMaxOffset() => focusMaxOffset;
    public float GetDeadZoneHeight() => deadZoneHeight;
    public float GetSmoothTime() => smoothTime;
    public Vector2 GetOffset() => offset;
}
