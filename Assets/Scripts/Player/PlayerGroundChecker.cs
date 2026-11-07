using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGroundChecker : MonoBehaviour
{
    private const float BoxHeight = 0.05f;

    [SerializeField] private PlayerSo playerData;

    [SerializeField] private Transform origin;        // Punto a la altura de los pies
    private float feetWidth;
    private float checkDistance;
    private LayerMask groundLayer;
    private Rigidbody2D body;
    private ContactFilter2D floorFilter;

    private bool isGrounded;
    public bool GetIsGrounded() => isGrounded;

    public event Action OnLanded;

    private void Awake()
    {
        feetWidth = playerData.GetFeetWidth();
        checkDistance = playerData.GetCheckDistance();
        groundLayer = playerData.GetGroundLayer();
        body = GetComponent<Rigidbody2D>();

        // Solo contactos que empujan hacia arriba, las paredes no cuentan
        floorFilter = new ContactFilter2D();
        floorFilter.SetLayerMask(groundLayer);
        floorFilter.SetNormalAngle(45f, 135f);
    }

    private void Reset()
    {
        origin = transform;
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;

        CheckGround();

        if (isGrounded && !wasGrounded) OnLanded?.Invoke();
    }

    // La caja de los pies da margen antes de tocar el piso. Es un poco mas angosta que el collider para no tomar una pared como piso.
    // Apoyado justo en la punta de una esquina la caja no llega, pero la fisica igual lo sostiene, por eso tambien vale el contacto
    private void CheckGround()
    {
        isGrounded = Physics2D.BoxCast(origin.position, GetBoxSize(), 0f, Vector2.down, checkDistance, groundLayer)
            || body.IsTouching(floorFilter);
    }

    // Si hay piso a esta distancia debajo de los pies
    public bool GetIsGroundNear(float distance)
    {
        return Physics2D.BoxCast(origin.position, GetBoxSize(), 0f, Vector2.down, distance, groundLayer);
    }

    private Vector2 GetBoxSize() => new Vector2(feetWidth, BoxHeight);

    private void OnDrawGizmosSelected()
    {
        if (origin == null || playerData == null) return;

        Gizmos.color = Application.isPlaying && isGrounded ? Color.green : Color.red;
        Vector2 size = new Vector2(playerData.GetFeetWidth(), BoxHeight + playerData.GetCheckDistance());
        Gizmos.DrawWireCube((Vector2)origin.position + Vector2.down * playerData.GetCheckDistance() * 0.5f, size);
    }
}
