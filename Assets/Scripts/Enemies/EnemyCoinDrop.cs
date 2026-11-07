using UnityEngine;

// Suelta monedas que saltan y caen al piso, solo la primera vez que muere en la partida.
// El enemigo no se destruye al reaparecer, asi el flag sigue y los reaparecidos no sueltan nada
[RequireComponent(typeof(EnemyHealth))]
public class EnemyCoinDrop : MonoBehaviour
{
    [SerializeField] private EnemySo enemyData;
    [SerializeField] private PickupSo pickupData;
    [Tooltip("Coin with a Rigidbody2D, each one is worth one coin")]
    [SerializeField] private Rigidbody2D coinPrefab;

    private EnemyHealth health;
    private bool hasDropped;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        health.OnDied += Drop;
    }

    private void OnDisable()
    {
        health.OnDied -= Drop;
    }

    private void Drop()
    {
        if (hasDropped) return;
        hasDropped = true;

        Vector3 position = transform.position + Vector3.up * pickupData.GetDropHeight();
        for (int i = 0; i < enemyData.GetCoinDrop(); i++)
        {
            Rigidbody2D coin = Instantiate(coinPrefab, position, Quaternion.identity);
            float side = Random.Range(-pickupData.GetDropSideImpulse(), pickupData.GetDropSideImpulse());
            float up = Random.Range(pickupData.GetDropMinUpImpulse(), pickupData.GetDropMaxUpImpulse());
            coin.AddForce(new Vector2(side, up), ForceMode2D.Impulse);
        }
    }
}
