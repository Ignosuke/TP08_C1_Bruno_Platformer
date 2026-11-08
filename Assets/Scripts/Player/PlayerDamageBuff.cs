using UnityEngine;

// El pickable de daño. Suma daño a todos los golpes un rato y tiñe al jugador mientras dura.
// Otro pickable reinicia el tiempo, no se acumula. Si hay invulnerabilidad su tinte celeste manda
[RequireComponent(typeof(PlayerAttacker))]
public class PlayerDamageBuff : MonoBehaviour
{
    [SerializeField] private PickupSo pickupData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerAttacker attacker;
    private PlayerInvulnerability invulnerability;
    private float buffCounter;

    private void Awake()
    {
        attacker = GetComponent<PlayerAttacker>();
        invulnerability = GetComponent<PlayerInvulnerability>();
    }

    /// <summary>Si el daño sigue activo, para volver a su tinte cuando termina la invulnerabilidad</summary>
    public bool GetIsActive() => buffCounter > 0f;

    public void Apply()
    {
        buffCounter = pickupData.GetDamageDuration();
        attacker.SetBonusDamage(pickupData.GetDamageBonus());
        if (invulnerability == null || !invulnerability.GetIsActive()) spriteRenderer.color = pickupData.GetDamageTint();
    }

    private void Update()
    {
        if (buffCounter <= 0f) return;

        buffCounter -= Time.deltaTime;
        if (buffCounter > 0f) return;

        attacker.SetBonusDamage(0);
        if (invulnerability == null || !invulnerability.GetIsActive()) spriteRenderer.color = Color.white;
    }
}
