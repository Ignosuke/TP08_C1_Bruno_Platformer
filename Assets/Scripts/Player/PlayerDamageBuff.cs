using UnityEngine;

// El pickable de daño. Suma daño a todos los golpes un rato y tiñe al jugador mientras dura.
// Otro pickable reinicia el tiempo, no se acumula
[RequireComponent(typeof(PlayerAttacker))]
public class PlayerDamageBuff : MonoBehaviour
{
    [SerializeField] private PickupSo pickupData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerAttacker attacker;
    private float buffCounter;

    private void Awake()
    {
        attacker = GetComponent<PlayerAttacker>();
    }

    public void Apply()
    {
        buffCounter = pickupData.GetDamageDuration();
        attacker.SetBonusDamage(pickupData.GetDamageBonus());
        spriteRenderer.color = pickupData.GetDamageTint();
    }

    private void Update()
    {
        if (buffCounter <= 0f) return;

        buffCounter -= Time.deltaTime;
        if (buffCounter > 0f) return;

        attacker.SetBonusDamage(0);
        spriteRenderer.color = Color.white;
    }
}
