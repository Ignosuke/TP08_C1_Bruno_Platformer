using UnityEngine;

// Todo lo que el jugador puede golpear (el muñeco de prueba ahora, los enemigos en la historia 004)
public interface IDamageable
{
    // hitDirection es hacia donde viaja el golpe (el atacante mirando al objetivo).
    // Devuelve true si se paro con un parry perfecto, el atacante cuerpo a cuerpo lo usa para quedar aturdido
    bool TakeDamage(int amount, Vector2 hitDirection);
}
