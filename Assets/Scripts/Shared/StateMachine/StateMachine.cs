using System;
using System.Collections.Generic;

// FSM generica, no es MonoBehaviour. Los estados no se cambian solos, la FSM revisa las transiciones cada frame
public class StateMachine
{
    private class Transition
    {
        public State from;
        public State to;
        public Func<bool> when;
    }

    private readonly List<Transition> transitions = new List<Transition>();
    private readonly List<Transition> anyTransitions = new List<Transition>();
    private readonly Stack<State> stack = new Stack<State>();     // Para pausas y menus encimados

    private State current;

    public State GetCurrent() => current;

    public void AddTransition(State from, State to, Func<bool> when)
    {
        transitions.Add(new Transition { from = from, to = to, when = when });
    }

    // Vale desde cualquier estado y se revisa antes que las demas, para muerte o daño
    public void AddAnyTransition(State to, Func<bool> when)
    {
        anyTransitions.Add(new Transition { to = to, when = when });
    }

    public void Update()
    {
        State next = FindNext();

        // Si gana el estado actual se queda, no lo vuelve a arrancar
        if (next != null && next != current) ChangeState(next);

        current?.Update();
    }

    // Gana la primera condicion que se cumpla, el orden en que se agregan es la prioridad
    private State FindNext()
    {
        foreach (Transition t in anyTransitions)
            if (t.when()) return t.to;

        foreach (Transition t in transitions)
            if (t.from == current && t.when()) return t.to;

        return null;
    }

    // Siempre entra de nuevo, aunque sea el mismo estado. Con null queda vacia
    public void ChangeState(State next)
    {
        current?.Exit();
        current = next;
        current?.Enter();
    }

    // El actual queda congelado en la pila, sin Exit
    public void PushState(State next)
    {
        stack.Push(current);
        current = next;
        current.Enter();
    }

    // Vuelve al que estaba antes, sea cual sea
    public void PopState()
    {
        if (stack.Count == 0) return;

        current?.Exit();
        current = stack.Pop();
    }
}
