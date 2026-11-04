using UnityEngine;
using UnityEngine.InputSystem;

// Solo para probar la vida sin enemigos, la tecla H le pega al jugador de frente como lo haria un enemigo.
// Pasa por TakeDamage, asi tambien sirve para probar el bloqueo y el parry
[RequireComponent(typeof(PlayerHealth))]
public class DebugDamageKey : MonoBehaviour
{
    [Tooltip("In half hearts, 2 = 1 heart")]
    [SerializeField] private int damage = 2;
    [Tooltip("Knockback strength of the test hit, 0 = no push")]
    [SerializeField] private float knockback = 6f;

    private PlayerHealth health;
    private PlayerMover mover;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        mover = GetComponent<PlayerMover>();
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.hKey.wasPressedThisFrame) return;

        // El golpe viaja hacia el jugador, contrario a donde mira
        health.TakeDamage(new DamageInfo(damage, new Vector2(-mover.GetFacing(), 0f), gameObject.layer, knockback));
    }
}
