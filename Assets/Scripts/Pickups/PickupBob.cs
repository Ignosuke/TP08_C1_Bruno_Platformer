using UnityEngine;

// Sube y baja el dibujo de a un pixel, asi el pickable se nota en el nivel.
// Solo mueve el Visual, la raiz y su trigger se quedan quietos
public class PickupBob : MonoBehaviour
{
    private const float PixelSize = 1f / 32f;      // Igual que los pixeles por unidad del arte

    [Tooltip("Child with the sprite, the only thing that moves")]
    [SerializeField] private Transform visual;
    [Tooltip("Pixels it goes up before coming back down")]
    [SerializeField] private int steps = 2;
    [Tooltip("Seconds between one pixel and the next")]
    [SerializeField] private float stepPause = 0.15f;
    [Tooltip("Seconds it waits at the top and at the bottom")]
    [SerializeField] private float endPause = 0.5f;

    private Vector3 startPosition;
    private int height;                 // Pixeles por encima del inicio
    private int direction = 1;
    private float waitCounter;

    private void Awake()
    {
        startPosition = visual.localPosition;
        waitCounter = Random.Range(0f, endPause);  // Cada uno arranca en otro momento, asi no se mueven todos juntos
    }

    private void Update()
    {
        waitCounter -= Time.deltaTime;
        if (waitCounter > 0f) return;

        height += direction;
        visual.localPosition = startPosition + Vector3.up * (height * PixelSize);

        // Arriba o abajo del todo da la vuelta y espera mas
        bool isAtEnd = height >= steps || height <= 0;
        if (isAtEnd) direction = -direction;
        waitCounter = isAtEnd ? endPause : stepPause;
    }
}
