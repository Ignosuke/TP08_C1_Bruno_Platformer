using UnityEngine;

// Conecta la vida y las curaciones del jugador con sus filas de iconos.
// Las filas no saben del jugador, asi el jefe (historia 005) puede tener su propia barra
public class PlayerHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerHealer healer;
    [SerializeField] private IconRow hearts;
    [SerializeField] private IconRow charges;

    private void OnEnable()
    {
        health.OnHealthChanged += hearts.Refresh;
        healer.OnChargesChanged += charges.Refresh;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= hearts.Refresh;
        healer.OnChargesChanged -= charges.Refresh;
    }

    // En Start porque la vida y las cargas se crean en el Awake del jugador
    private void Start()
    {
        hearts.Refresh(health.GetCurrent(), health.GetMax());
        charges.Refresh(healer.GetCharges(), healer.GetMaxCharges());
    }
}
