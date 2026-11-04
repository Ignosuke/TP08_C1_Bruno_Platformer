using UnityEngine;

// Cuerpo que lastima al tocarlo, solo en los demonios. Va en un hijo con un trigger en la capa EnemyDamage,
// que choca con el jugador aunque la capa Enemies lo atraviese. Muerto deja de lastimar
[RequireComponent(typeof(Collider2D))]
public class EnemyContactDamage : MonoBehaviour, IDamageSource
{
    [SerializeField] private EnemySo enemyData;
    [SerializeField] private Health health;            // La del enemigo, en la raiz

    private Collider2D trigger;

    public int Damage => enemyData.GetContactDamage();
    public float Knockback => enemyData.GetContactKnockback();

    private void Awake()
    {
        trigger = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        trigger.enabled = true;
        health.OnDied += TurnOff;
    }

    private void OnDisable()
    {
        health.OnDied -= TurnOff;
    }

    private void TurnOff()
    {
        trigger.enabled = false;
    }
}
