using UnityEngine;

// Cierra la entrada de la arena cuando empieza la pelea, asi no se puede escapar ni curar en el checkpoint.
// Se abre cuando el jugador reaparece o cuando vence al jefe
public class ArenaGate : MonoBehaviour
{
    [SerializeField] private BossBrain brain;
    [Tooltip("Wall that blocks the arena entrance, assigned by the builder")]
    [SerializeField] private GameObject wall;

    // En el editor el muro queda visible para ubicarlo, al jugar arranca abierto
    private void Awake()
    {
        Open();
    }

    private void OnEnable()
    {
        brain.OnFightStarted += Close;
        brain.OnFightReset += Open;
        brain.OnDefeated += Open;
    }

    private void OnDisable()
    {
        brain.OnFightStarted -= Close;
        brain.OnFightReset -= Open;
        brain.OnDefeated -= Open;
    }

    private void Close() => wall.SetActive(true);

    private void Open() => wall.SetActive(false);
}
