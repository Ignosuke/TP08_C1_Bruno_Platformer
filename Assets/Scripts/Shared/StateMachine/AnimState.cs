using UnityEngine;

// Estado que al entrar reproduce el estado del Animator con su mismo nombre, desde cero salvo que un hijo cambie startTime
public class AnimState : State
{
    protected string stateName;
    protected float startTime;          // Fraccion del clip donde arranca, 0 = el principio

    private readonly Animator animator;

    public AnimState(Animator animator, string stateName)
    {
        this.animator = animator;
        this.stateName = stateName;
    }

    public override void Enter()
    {
        animator.Play(stateName, 0, startTime);
    }

    // Cambia el clip que va a reproducir la proxima vez que entre
    public void SetStateName(string newStateName)
    {
        stateName = newStateName;
    }

    public bool GetIsFinished()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName) && info.normalizedTime >= 1f;
    }
}
