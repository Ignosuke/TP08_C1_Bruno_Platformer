using UnityEngine;

// Sigue al jugador con zona muerta y limites por area. Lleva su propia posicion de seguimiento
// y le suma encima el desvio de CameraShake, asi el temblor no altera el seguimiento ni los limites.
// Por el canal de foco puede mirar otra cosa un rato, como el jefe en su cambio de fase
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CameraSo cameraData;
    [SerializeField] private Transform target;
    [SerializeField] private AreaTracker areaTracker;   // Los limites son los del area donde esta el jugador
    [SerializeField] private PlayerHealth playerHealth; // Al reaparecer la camara salta al checkpoint
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private CameraFocusChannel cameraFocusChannel;

    private float halfWidth;
    private float halfHeight;
    private float smoothTime;
    private Vector2 offset;

    private Camera cam;
    private Bounds areaBounds;
    private bool hasBounds;

    private Vector3 smoothVelocity;
    private Vector3 followPosition;     // Donde estaria la camara sin temblor
    private Transform focus;            // Si no es null mira esto en vez del jugador

    private void Awake()
    {
        cam = GetComponent<Camera>();

        halfWidth = cameraData.GetDeadZoneWidth() * 0.5f;
        halfHeight = cameraData.GetDeadZoneHeight() * 0.5f;
        smoothTime = cameraData.GetSmoothTime();
        offset = cameraData.GetOffset();
        followPosition = transform.position;
    }

    private void OnEnable()
    {
        areaTracker.OnAreaChanged += SetArea;
        playerHealth.OnRespawned += SnapAfterRespawn;
        if (cameraFocusChannel != null) cameraFocusChannel.OnRaised += SetFocus;
    }

    private void OnDisable()
    {
        areaTracker.OnAreaChanged -= SetArea;
        playerHealth.OnRespawned -= SnapAfterRespawn;
        if (cameraFocusChannel != null) cameraFocusChannel.OnRaised -= SetFocus;
    }

    // Con null vuelve al jugador, el SmoothDamp hace el paneo de ida y de vuelta
    private void SetFocus(Transform value)
    {
        focus = value;
    }

    // Al cambiar de area el SmoothDamp lleva la camara a los nuevos limites, eso hace el paneo entre areas
    private void SetArea(Area area)
    {
        bool isFirstArea = !hasBounds;

        areaBounds = area.GetBounds();
        hasBounds = true;

        // La primera vez salta directo, si no arrancaria desde donde quedo la camara en la escena
        if (isFirstArea && target != null) SnapToTarget();
    }

    // Primero se actualiza el area para que los limites ya sean los del checkpoint, si no paneria por todo el mapa
    private void SnapAfterRespawn()
    {
        areaTracker.Refresh();
        SnapToTarget();
    }

    private void SnapToTarget()
    {
        followPosition = ClampToBounds(new Vector3(target.position.x + offset.x, target.position.y + offset.y, followPosition.z));
        smoothVelocity = Vector3.zero;
        ApplyPosition();
    }

    // En LateUpdate para asegurar que el jugador ya termino de moverse este frame
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = focus != null ? ResolveFocusPosition() : ClampToBounds(ResolveDesiredPosition());

        followPosition = Vector3.SmoothDamp(followPosition, desired, ref smoothVelocity, smoothTime);
        ApplyPosition();
    }

    private void ApplyPosition()
    {
        transform.position = followPosition + cameraShake.GetOffset();
    }

    // Mientras el jugador este dentro del rectangulo, el destino es la posicion actual y no se mueve
    private Vector3 ResolveDesiredPosition()
    {
        Vector2 center = GetDeadZoneCenter();
        Vector2 delta = (Vector2)target.position - center;

        float x = center.x;
        float y = center.y;

        if (Mathf.Abs(delta.x) > halfWidth)
            x = target.position.x - Mathf.Sign(delta.x) * halfWidth;

        if (Mathf.Abs(delta.y) > halfHeight)
            y = target.position.y - Mathf.Sign(delta.y) * halfHeight;

        return new Vector3(x + offset.x, y + offset.y, followPosition.z);
    }

    // Centrado en el foco dentro de los limites. Si los limites lo dejan muy al costado se pasan, asi nunca queda en el borde
    private Vector3 ResolveFocusPosition()
    {
        Vector3 position = ClampToBounds(new Vector3(focus.position.x + offset.x, focus.position.y + offset.y, followPosition.z));
        float maxOffset = cam.orthographicSize * cam.aspect * cameraData.GetFocusMaxOffset();
        position.x = Mathf.Clamp(position.x, focus.position.x - maxOffset, focus.position.x + maxOffset);
        return position;
    }

    private Vector2 GetDeadZoneCenter() => (Vector2)followPosition - offset;

    private Vector3 ClampToBounds(Vector3 position)
    {
        if (!hasBounds) return position;

        // Recalculado cada frame por si cambia la resolucion de la ventana
        float viewHalfHeight = cam.orthographicSize;
        float viewHalfWidth = viewHalfHeight * cam.aspect;

        float x = ClampAxis(position.x,
            areaBounds.min.x + viewHalfWidth, areaBounds.max.x - viewHalfWidth, areaBounds.center.x);

        float y = ClampAxis(position.y,
            areaBounds.min.y + viewHalfHeight, areaBounds.max.y - viewHalfHeight, areaBounds.center.y);

        return new Vector3(x, y, position.z);
    }

    // Si el area es mas chica que la pantalla en ese eje, la centramos en vez de clampear
    private float ClampAxis(float value, float min, float max, float center)
    {
        if (min > max) return center;

        return Mathf.Clamp(value, min, max);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraData == null) return;

        Vector2 center = (Vector2)transform.position - cameraData.GetOffset();
        Vector2 size = new Vector2(cameraData.GetDeadZoneWidth(), cameraData.GetDeadZoneHeight());

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
