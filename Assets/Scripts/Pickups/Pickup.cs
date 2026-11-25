using UnityEngine;

public enum PickupType { Coin, Pouch, Heal, Damage, Invulnerability }

// Una moneda, una bolsa o un pickable. Al tocarlo el jugador se aplica, suena y desaparece.
// No vuelve al reaparecer, cada uno se junta una vez por partida
public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupSo pickupData;
    [SerializeField] private PickupType type;

    private bool isTaken;               // Destroy espera al final del frame, asi no se junta dos veces

    // Se reconoce al jugador por sus componentes, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTaken) return;
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null || health.GetIsDead()) return;

        isTaken = true;
        other.GetComponent<PlayerAudio>().Play(Apply(health));
        Destroy(gameObject);
    }

    // Devuelve el sonido del pickable
    private AudioClip Apply(PlayerHealth health)
    {
        switch (type)
        {
            case PickupType.Coin:
                health.GetComponent<PlayerWallet>().Add(pickupData.GetCoinValue());
                return pickupData.GetCoinClip();
            case PickupType.Pouch:
                health.GetComponent<PlayerWallet>().Add(pickupData.GetPouchValue());
                return pickupData.GetPouchClip();
            case PickupType.Heal:
                health.Heal(pickupData.GetHealAmount());
                return pickupData.GetHealClip();
            case PickupType.Damage:
                health.GetComponent<PlayerDamageBuff>().Apply();
                return pickupData.GetDamageClip();
            // Suggestion: la invulnerabilidad queda en el default, asi cualquier PickupType nuevo que se agregue al enum sin su case
            // daria invulnerabilidad. Usa case PickupType.Invulnerability: y deja el default para un error. Con herencia
            // (Pickup abstracto y un hijo por tipo, como en clase) este switch desaparece.
            default:
                health.GetComponent<PlayerInvulnerability>().Apply();
                return pickupData.GetInvulnerabilityClip();
        }
    }
}
