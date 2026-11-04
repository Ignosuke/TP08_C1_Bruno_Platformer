using UnityEngine;

// Va en el campamento del herrero y marca donde reaparece el jugador al morir.
// Su trigger recarga las curaciones cuando el jugador pasa por el campamento
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Where the player appears after dying. Move this child to adjust it")]
    [SerializeField] private Transform respawnPoint;

    public Vector3 GetRespawnPosition()
    {
        return respawnPoint.position;
    }

    // Se reconoce al jugador por su componente, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealer healer = other.GetComponent<PlayerHealer>();
        if (healer != null) healer.RefillCharges();
    }
}
