using System;
using UnityEngine;

// Va en el campamento del herrero. Al tocarlo se activa, desde ahi el jugador reaparece aca al morir.
// Su trigger tambien recarga las curaciones cada vez que el jugador pasa por el campamento
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Where the player appears after dying. Move this child to adjust it")]
    [SerializeField] private Transform respawnPoint;

    private bool isActive;

    public event Action OnActivated;    // Una sola vez, para el indicador de activado

    public Vector3 GetRespawnPosition()
    {
        return respawnPoint.position;
    }

    // Se reconoce al jugador por sus componentes, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealer healer = other.GetComponent<PlayerHealer>();
        if (healer != null) healer.RefillCharges();

        PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
        if (respawner == null || isActive) return;

        isActive = true;
        respawner.SetCheckpoint(this);
        OnActivated?.Invoke();
    }

    public bool GetIsActive() => isActive;
}
