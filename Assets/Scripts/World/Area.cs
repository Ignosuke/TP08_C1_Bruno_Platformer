using UnityEngine;

// Un area del nivel. El BoxCollider2D marca sus limites, la camara no sale de ellos
[RequireComponent(typeof(BoxCollider2D))]
public class Area : MonoBehaviour
{
    [SerializeField] private AreaSo areaData;

    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    // Solo en x e y, los bounds del collider tienen su propia z
    public bool Contains(Vector2 point)
    {
        Bounds bounds = box.bounds;
        return point.x >= bounds.min.x && point.x <= bounds.max.x && point.y >= bounds.min.y && point.y <= bounds.max.y;
    }

    public Bounds GetBounds() => box.bounds;
    public AreaSo GetAreaData() => areaData;

    // Los limites siempre visibles en la escena, para pintar los tiles adentro
    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(0.3f, 1f, 0.5f, 0.6f);
        Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
    }
}
