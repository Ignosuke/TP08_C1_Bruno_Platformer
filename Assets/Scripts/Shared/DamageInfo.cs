using UnityEngine;

// Los datos de un golpe. Viajan en TakeDamage y en OnDamaged, y cada sistema decide si el golpe le incumbe
public readonly struct DamageInfo
{
    public readonly int Amount;
    public readonly Vector2 Direction;  // Desde la fuente hacia quien recibe
    public readonly int SourceLayer;
    public readonly float Knockback;    // Fuerza del empujon, 0 no empuja
    public readonly bool IsMelee;       // Golpe cuerpo a cuerpo de MeleeHitbox, no contacto ni proyectil

    public DamageInfo(int amount, Vector2 direction, int sourceLayer, float knockback, bool isMelee = false)
    {
        Amount = amount;
        Direction = direction;
        SourceLayer = sourceLayer;
        Knockback = knockback;
        IsMelee = isMelee;
    }
}
