using UnityEngine;

// Un numero entero entre 0 y un maximo, como la vida o las cargas de curacion.
// Es una clase comun y no un MonoBehaviour, asi la cuenta se puede probar sin abrir el juego
public class ResourcePool
{
    private int current;
    private int max;

    public ResourcePool(int max)
    {
        this.max = Mathf.Max(0, max);
        current = this.max;
    }

    // Devuelve cuanto se saco de verdad, nunca baja de 0
    public int Remove(int amount)
    {
        int removed = Mathf.Clamp(amount, 0, current);
        current -= removed;
        return removed;
    }

    // Devuelve cuanto se sumo de verdad, nunca pasa del maximo
    public int Add(int amount)
    {
        int added = Mathf.Clamp(amount, 0, max - current);
        current += added;
        return added;
    }

    public void Refill()
    {
        current = max;
    }

    public int GetCurrent() => current;
    public int GetMax() => max;
    public bool GetIsEmpty() => current <= 0;
    public bool GetIsFull() => current >= max;
}
