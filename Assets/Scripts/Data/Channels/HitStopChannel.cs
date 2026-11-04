using System;
using UnityEngine;

// Canal para pedir un hitstop sin conocer a HitStop. Lo levantan los golpes y lo escucha HitStop en la escena
[CreateAssetMenu(fileName = "HitStopChannel", menuName = "Data/Channels/Hit Stop")]
public class HitStopChannel : ScriptableObject
{
    public event Action<float> OnRaised;    // Manda la duracion en segundos reales

    // Duracion 0 no hace nada
    public void Raise(float duration)
    {
        if (duration > 0f) OnRaised?.Invoke(duration);
    }
}
