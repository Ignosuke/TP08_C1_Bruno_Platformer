using System;
using UnityEngine;

// Canal para pedir un temblor de camara sin conocer a la camara. Lo escucha CameraShake
[CreateAssetMenu(fileName = "CameraShakeChannel", menuName = "Data/Channels/Camera Shake")]
public class CameraShakeChannel : ScriptableObject
{
    public event Action<float, float> OnRaised;    // Manda la intensidad y la duracion

    // Intensidad o duracion 0 no hace nada
    public void Raise(float magnitude, float duration)
    {
        if (magnitude > 0f && duration > 0f) OnRaised?.Invoke(magnitude, duration);
    }
}
