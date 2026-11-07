using UnityEngine;
using UnityEngine.InputSystem;

// El mercader. Con el jugador cerca muestra el aviso, al apretar interactuar abre la tienda y congela al jugador hasta cerrarla.
// Un golpe cierra la tienda, y si lo empuja el control lo devuelve PlayerKnockback al terminar el empujon
[RequireComponent(typeof(Shop))]
public class Merchant : MonoBehaviour
{
    [SerializeField] private ShopSo shopData;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private ShopPanel panel;
    [Tooltip("HUD text shown while the player is in range")]
    [SerializeField] private GameObject prompt;

    private InputActionReference interactAction;
    private Rigidbody2D playerBody;
    private Behaviour[] playerControls;
    private bool isInRange;
    private bool isClosingOnHit;

    private void Awake()
    {
        interactAction = shopData.GetInteractAction();
        playerBody = player.GetComponent<Rigidbody2D>();

        // Los mismos controles que apaga PlayerRespawner al morir
        playerControls = new Behaviour[]
        {
            player.GetComponent<PlayerMover>(), player.GetComponent<PlayerJumper>(), player.GetComponent<PlayerDasher>(),
            player.GetComponent<PlayerAttacker>(), player.GetComponent<PlayerDefender>(), player.GetComponent<PlayerHealer>(),
        };
    }

    // started y no performed, la accion Interact tiene la interaccion Hold y asi abre apenas se aprieta
    private void OnEnable()
    {
        interactAction.action.started += OnInteractStarted;
        interactAction.action.Enable();
        panel.OnClosed += Release;
        player.OnDamaged += CloseOnHit;
    }

    private void OnDisable()
    {
        interactAction.action.started -= OnInteractStarted;
        interactAction.action.Disable();
        panel.OnClosed -= Release;
        player.OnDamaged -= CloseOnHit;
    }

    // Si la tienda ya esta abierta la cierra el panel con la misma tecla
    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (!isInRange || panel.GetIsOpen() || player.GetIsDead() || !playerControls[0].enabled) return;

        SetPlayerFrozen(true);
        panel.Open();
    }

    private void Update()
    {
        prompt.SetActive(isInRange && !panel.GetIsOpen());
    }

    private void Release()
    {
        if (isClosingOnHit || player.GetIsDead()) return;
        SetPlayerFrozen(false);
    }

    // Mismo criterio que PlayerKnockback para saber si hay empujon
    private void CloseOnHit(DamageInfo info)
    {
        if (!panel.GetIsOpen()) return;

        isClosingOnHit = info.Knockback > 0f && info.Direction.x != 0f;
        panel.Close();
        isClosingOnHit = false;
    }

    // Quieto en x, igual que en el cambio de fase del jefe
    private void SetPlayerFrozen(bool isFrozen)
    {
        foreach (Behaviour control in playerControls) control.enabled = !isFrozen;
        if (isFrozen) playerBody.linearVelocity = new Vector2(0f, playerBody.linearVelocity.y);
    }

    // Se reconoce al jugador por sus componentes, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() == player) isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() == player) isInRange = false;
    }
}
