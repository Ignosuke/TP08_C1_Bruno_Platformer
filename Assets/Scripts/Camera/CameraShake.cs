using UnityEngine;

// Temblor de camara que escucha el CameraShakeChannel. Solo calcula un desvio al azar que se apaga con el tiempo,
// CameraFollow lo suma encima de su posicion, asi el seguimiento y los limites no se tocan y nunca se corre
public class CameraShake : MonoBehaviour
{
    [SerializeField] private CameraShakeChannel cameraShakeChannel;

    private float magnitude;
    private float duration;
    private float shakeCounter;
    private Vector3 offset;

    private void OnEnable()
    {
        cameraShakeChannel.OnRaised += Shake;
    }

    private void OnDisable()
    {
        cameraShakeChannel.OnRaised -= Shake;
        shakeCounter = 0f;
        offset = Vector3.zero;
    }

    // Si ya hay un temblor mas fuerte en curso se queda ese
    private void Shake(float newMagnitude, float newDuration)
    {
        if (shakeCounter > 0f && GetCurrentMagnitude() > newMagnitude) return;

        magnitude = newMagnitude;
        duration = newDuration;
        shakeCounter = newDuration;
    }

    // Tiempo real, asi tiembla aunque el hitstop tenga el juego parado
    private void Update()
    {
        if (shakeCounter <= 0f)
        {
            offset = Vector3.zero;
            return;
        }

        shakeCounter -= Time.unscaledDeltaTime;
        offset = (Vector3)(Random.insideUnitCircle * GetCurrentMagnitude());
    }

    private float GetCurrentMagnitude() => magnitude * Mathf.Max(0f, shakeCounter) / duration;

    public Vector3 GetOffset() => offset;
}
