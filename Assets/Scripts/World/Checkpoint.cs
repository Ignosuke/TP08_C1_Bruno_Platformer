using System;
using UnityEngine;

// Va al lado de cada torii del nivel. Al tocarlo se activa, desde ahi el jugador reaparece aca al morir.
// Como un banco, cada vez que el jugador pasa por el torii le llena la vida pero no las curaciones
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Where the player appears after dying. Move this child to adjust it")]
    [SerializeField] private Transform respawnPoint;

    private bool isActive;

    public event Action OnActivated;    // Una sola vez, para el indicador de activado

    public Vector3 GetRespawnPosition() => respawnPoint.position;

    // Se reconoce al jugador por sus componentes, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null && !health.GetIsDead()) health.Heal(health.GetMax());

        PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
        if (respawner == null || isActive) return;

        isActive = true;
        respawner.SetCheckpoint(this);
        OnActivated?.Invoke();
    }
}
