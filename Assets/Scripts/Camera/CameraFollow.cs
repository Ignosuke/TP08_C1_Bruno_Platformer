using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CameraSo cameraData;
    [SerializeField] private Transform target;
    [SerializeField] private AreaTracker areaTracker;   // Los limites son los del area donde esta el jugador
    [SerializeField] private PlayerHealth playerHealth; // Al reaparecer la camara salta al checkpoint

    private float halfWidth;
    private float halfHeight;
    private float smoothTime;
    private Vector2 offset;

    private Camera cam;
    private Bounds areaBounds;
    private bool hasBounds;

    private Vector3 smoothVelocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        halfWidth = cameraData.GetDeadZoneWidth() * 0.5f;
        halfHeight = cameraData.GetDeadZoneHeight() * 0.5f;
        smoothTime = cameraData.GetSmoothTime();
        offset = cameraData.GetOffset();
    }

    private void OnEnable()
    {
        areaTracker.OnAreaChanged += SetArea;
        playerHealth.OnRespawned += SnapAfterRespawn;
    }

    private void OnDisable()
    {
        areaTracker.OnAreaChanged -= SetArea;
        playerHealth.OnRespawned -= SnapAfterRespawn;
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
        transform.position = ClampToBounds(new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z));
        smoothVelocity = Vector3.zero;
    }

    // En LateUpdate para asegurar que el jugador ya termino de moverse este frame
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = ClampToBounds(ResolveDesiredPosition());

        transform.position = Vector3.SmoothDamp(
            transform.position, desired, ref smoothVelocity, smoothTime);
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

        return new Vector3(x + offset.x, y + offset.y, transform.position.z);
    }

    private Vector2 GetDeadZoneCenter()
    {
        return (Vector2)transform.position - offset;
    }

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
