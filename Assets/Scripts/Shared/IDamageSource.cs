// Lo implementa cualquier cosa que haga daño al tocarla, como el cuerpo de un demonio o una flecha
public interface IDamageSource
{
    int Damage { get; }
    float Knockback { get; }    // Fuerza del empujon, 0 no empuja
}
