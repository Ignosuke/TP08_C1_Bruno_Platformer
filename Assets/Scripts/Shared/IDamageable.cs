// Todo lo que se puede golpear, el jugador, los enemigos y el muñeco de prueba.
// Devuelve true si se paro con un parry perfecto, el atacante cuerpo a cuerpo lo usa para quedar aturdido
public interface IDamageable
{
    bool TakeDamage(DamageInfo info);
}
