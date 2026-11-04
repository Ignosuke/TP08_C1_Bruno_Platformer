using UnityEngine;

// Congela el juego unas centesimas cuando un golpe conecta. Escucha el HitStopChannel, va en la camara de la escena.
// Usa un timeScale casi cero y no cero, asi se distingue de una pausa (timeScale 0) y nunca la rompe
public class HitStop : MonoBehaviour
{
    private const float FrozenScale = 0.0001f;

    [SerializeField] private HitStopChannel hitStopChannel;

    private float stopCounter;
    private float savedScale = 1f;

    private void OnEnable()
    {
        hitStopChannel.OnRaised += Stop;
    }

    private void OnDisable()
    {
        hitStopChannel.OnRaised -= Stop;
        stopCounter = 0f;
        Restore();
    }

    // Si dos pedidos se pisan queda el mas largo. En pausa no hace nada
    private void Stop(float duration)
    {
        if (Time.timeScale == 0f) return;

        if (Time.timeScale != FrozenScale) savedScale = Time.timeScale;
        stopCounter = Mathf.Max(stopCounter, duration);
        Time.timeScale = FrozenScale;
    }

    // Tiempo real, el escalado esta casi parado
    private void Update()
    {
        if (stopCounter > 0f) stopCounter -= Time.unscaledDeltaTime;
        if (stopCounter <= 0f) Restore();
    }

    // Solo vuelve si sigue congelado por nosotros, si alguien pauso o cambio la escala se respeta
    private void Restore()
    {
        if (Time.timeScale == FrozenScale) Time.timeScale = savedScale;
    }
}
