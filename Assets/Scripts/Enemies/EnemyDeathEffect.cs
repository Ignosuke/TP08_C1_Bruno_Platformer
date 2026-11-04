using UnityEngine;

// Suelta la explosion de particulas cuando el enemigo muere
public class EnemyDeathEffect : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private ParticleSystem burst;

    private void OnEnable()
    {
        health.OnDied += Play;
    }

    private void OnDisable()
    {
        health.OnDied -= Play;
    }

    private void Play()
    {
        burst.Play();
    }
}
