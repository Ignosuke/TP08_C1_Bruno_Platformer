using UnityEngine;

// Una capa del fondo. Sigue a la camara segun su factor y se repite a lo ancho sin fin.
// Orden 100 para correr despues de CameraFollow y usar la posicion de la camara de este mismo frame
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private ParallaxSo parallaxData;
    [SerializeField] private int layerIndex;     // Posicion de esta capa en el array del SO
    [SerializeField] private Camera cam;

    private float factor;
    private SpriteRenderer spriteRenderer;
    private Vector2 spriteSize;

    private void Awake()
    {
        factor = parallaxData.GetLayerFactor(layerIndex);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteSize = spriteRenderer.sprite.bounds.size;
    }

    private void LateUpdate()
    {
        // Cada frame porque la Pixel Perfect Camera cambia el tamaño de la vista segun la resolucion
        float viewHeight = cam.orthographicSize * 2f;
        float viewWidth = viewHeight * cam.aspect;
        float scale = viewHeight / spriteSize.y;
        float tileWidth = spriteSize.x * scale;

        // Escala pareja para cubrir el alto de la vista, y a lo ancho una vista mas dos sprites de margen
        transform.localScale = new Vector3(scale, scale, 1f);
        spriteRenderer.size = new Vector2(viewWidth / scale + spriteSize.x * 2f, spriteSize.y);

        // El dibujo se repite cada tileWidth, asi que solo importa el resto de la division y la capa nunca se aleja de la camara
        Vector3 camPosition = cam.transform.position;
        float x = camPosition.x - Mathf.Repeat(camPosition.x * (1f - factor), tileWidth);
        transform.position = new Vector3(x, camPosition.y, transform.position.z);
    }
}
