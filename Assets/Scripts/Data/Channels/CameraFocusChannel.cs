using System;
using UnityEngine;

// Canal para que la camara mire algo que no es el jugador sin conocer a la camara. Lo escucha CameraFollow
[CreateAssetMenu(fileName = "CameraFocusChannel", menuName = "Data/Channels/Camera Focus")]
public class CameraFocusChannel : ScriptableObject
{
    public event Action<Transform> OnRaised;      // Con null vuelve a seguir al jugador

    public void Focus(Transform target) => OnRaised?.Invoke(target);

    public void Clear() => OnRaised?.Invoke(null);
}
