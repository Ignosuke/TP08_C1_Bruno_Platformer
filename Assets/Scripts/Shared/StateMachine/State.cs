// Base de todos los estados, cada uno pisa solo lo que necesita
public abstract class State
{
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
