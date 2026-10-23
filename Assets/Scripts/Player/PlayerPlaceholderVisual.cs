using UnityEngine;

// Solo para probar sin arte: pinta el cuadrado segun lo que hace el jugador y muestra la hitbox del golpe.
// Se borra cuando lleguen los sprites.
public class PlayerPlaceholderVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer hitboxVisual;
    [SerializeField] private float hitboxShowTime = 0.1f;

    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;

    private float hitboxCounter;

    private void Awake()
    {
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
    }

    private void OnEnable()
    {
        attacker.OnHitboxChecked += ShowHitbox;
    }

    private void OnDisable()
    {
        attacker.OnHitboxChecked -= ShowHitbox;
    }

    private void Update()
    {
        if (dasher.GetIsDashing()) body.color = Color.cyan;
        else if (defender.GetIsParrying()) body.color = Color.white;
        else if (defender.GetIsDefending()) body.color = Color.blue;
        else if (attacker.GetIsAttacking()) body.color = new Color(1f, 0.6f, 0.2f);
        else body.color = Color.gray;

        hitboxCounter -= Time.deltaTime;
        hitboxVisual.enabled = hitboxCounter > 0f;
    }

    private void ShowHitbox()
    {
        hitboxCounter = hitboxShowTime;
        hitboxVisual.transform.position = attacker.GetHitboxCenter();
        Vector2 size = attacker.GetHitboxSize();
        hitboxVisual.transform.localScale = new Vector3(size.x, size.y, 1f);
    }
}
