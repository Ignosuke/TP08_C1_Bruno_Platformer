using UnityEngine;

// Todo lo que el jugador puede golpear (el muñeco de prueba ahora, los enemigos en la historia 004)
public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitDirection);
}
