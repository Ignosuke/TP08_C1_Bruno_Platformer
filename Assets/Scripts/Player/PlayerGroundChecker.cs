using System;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    private const float BoxHeight = 0.05f;

    [SerializeField] private PlayerSo playerData;

    [SerializeField] private Transform origin;        // Punto a la altura de los pies
    private float feetWidth;            // Ancho del personaje a la altura de los pies
    private float checkDistance;        // Distancia del raycast para detectar el suelo
    private LayerMask groundLayer;

    private bool isGrounded;
    public bool GetIsGrounded() => isGrounded;

    public event Action OnLanded;

    private void Awake()
    {
        feetWidth = playerData.GetFeetWidth();
        checkDistance = playerData.GetCheckDistance();
        groundLayer = playerData.GetGroundLayer();
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

    // Una caja del ancho de los pies en vez de rayos sueltos, asi apoyado en una esquina no queda ningun hueco sin revisar.
    // Un poco mas angosta que el collider para no tomar una pared de costado como piso
    private void CheckGround()
    {
        isGrounded = Physics2D.BoxCast(origin.position, GetBoxSize(), 0f, Vector2.down, checkDistance, groundLayer);
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
