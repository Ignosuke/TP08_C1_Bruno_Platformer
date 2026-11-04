using UnityEngine;

// Sabe donde esta el jugador. Lo usan el movimiento, el ataque y el reinicio
public class EnemySight : MonoBehaviour
{
    [SerializeField] private EnemySo enemyData;
    [Tooltip("The player in the scene, assigned by the builder")]
    [SerializeField] private PlayerHealth player;

    // Lo ve si esta vivo, cerca y mas o menos a la misma altura
    public bool CanSeePlayer()
    {
        if (player == null || player.GetIsDead()) return false;

        Vector2 offset = player.transform.position - transform.position;
        return Mathf.Abs(offset.x) <= enemyData.GetDetectionRange() && Mathf.Abs(offset.y) <= enemyData.GetVerticalTolerance();
    }

    // Positivo si el jugador esta a la derecha
    public float GetPlayerDistanceX()
    {
        return player.transform.position.x - transform.position.x;
    }

    public PlayerHealth GetPlayer() => player;
}
