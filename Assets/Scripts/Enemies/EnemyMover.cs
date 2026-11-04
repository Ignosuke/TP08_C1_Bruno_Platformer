using UnityEngine;

// Base del movimiento de los enemigos, como el WormMover del TP anterior. Rayos de piso y pared desde frontCheck,
// pausa antes de girar y frontCheck espejado en vez de rotar el objeto, asi la barra de vida no se da vuelta.
// Cada hijo decide a donde ir en UpdateMovement. EnemyBrain lo apaga mientras ataca, sufre o muere
[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyMover : MonoBehaviour
{
    [SerializeField] protected EnemySo enemyData;
    [SerializeField] private Transform frontCheck;      // Adelante, un poco arriba de los pies
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private int direction = 1;                          // 1 derecha, -1 izquierda
    private int startDirection;
    private float pauseCounter;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startDirection = direction;
        SetDirection(direction);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    // Apagado no se mueve, asi el brain lo frena con solo apagarlo
    private void OnDisable()
    {
        Stop();
        pauseCounter = 0f;
    }

    protected abstract void UpdateMovement();

    // Camina hasta una pared o un borde, se queda quieto la pausa y recien al final gira
    protected void Patrol(float speed)
    {
        if (pauseCounter > 0f)
        {
            Stop();
            pauseCounter -= Time.fixedDeltaTime;
            if (pauseCounter <= 0f) Flip();
            return;
        }

        if (IsBlocked())
        {
            pauseCounter = enemyData.GetTurnPauseDuration();
            Stop();
            return;
        }

        Move(speed);
    }

    // Se le acaba el piso o tiene una pared enfrente
    protected bool IsBlocked()
    {
        return !HasGroundAhead() || HasWallAhead();
    }

    private bool HasGroundAhead()
    {
        return Physics2D.Raycast(frontCheck.position, Vector2.down, enemyData.GetEdgeCheckDistance(), enemyData.GetGroundLayer());
    }

    private bool HasWallAhead()
    {
        return Physics2D.Raycast(frontCheck.position, Vector2.right * direction, enemyData.GetWallCheckDistance(), enemyData.GetGroundLayer());
    }

    public void Move(float speed)
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Flip() => SetDirection(-direction);

    public void SetDirection(int value)
    {
        direction = value;

        // Las hojas pueden estar dibujadas mirando a la izquierda
        spriteRenderer.flipX = (direction < 0) != enemyData.GetSpriteFacesLeft();

        Vector3 local = frontCheck.localPosition;
        local.x = Mathf.Abs(local.x) * direction;
        frontCheck.localPosition = local;
    }

    // Mira hacia el lado del numero, 0 lo deja como esta
    public void FaceTowards(float distance)
    {
        if (distance != 0f) SetDirection(distance > 0f ? 1 : -1);
    }

    // Vuelve a mirar como al empezar la escena
    public void ResetMover()
    {
        pauseCounter = 0f;
        SetDirection(startDirection);
    }

    public int GetDirection() => direction;

    private void OnDrawGizmosSelected()
    {
        if (enemyData == null || frontCheck == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(frontCheck.position, frontCheck.position + Vector3.down * enemyData.GetEdgeCheckDistance());

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(frontCheck.position, frontCheck.position + Vector3.right * direction * enemyData.GetWallCheckDistance());
    }
}
