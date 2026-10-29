using UnityEngine;

// Valores de la camara que sigue al jugador
[CreateAssetMenu(fileName = "CameraSo", menuName = "Data/Camera")]
public class CameraSo : ScriptableObject
{
    [SerializeField] private float deadZoneWidth = 1.5f;    // Rectangulo donde el jugador se mueve sin mover la camara
    [SerializeField] private float deadZoneHeight = 2f;
    [SerializeField] private float smoothTime = 0.15f;      // Cuanto tarda en alcanzar al jugador
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);  // Hacia arriba para ver mas de lo que viene que del piso

    public float GetDeadZoneWidth() => deadZoneWidth;
    public float GetDeadZoneHeight() => deadZoneHeight;
    public float GetSmoothTime() => smoothTime;
    public Vector2 GetOffset() => offset;
}
