using UnityEngine;

// Va en el campamento del herrero y marca donde reaparece el jugador. La historia 003 lo usa al morir
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Where the player appears after dying. Move this child to adjust it")]
    [SerializeField] private Transform respawnPoint;

    public Vector3 GetRespawnPosition()
    {
        return respawnPoint.position;
    }
}
