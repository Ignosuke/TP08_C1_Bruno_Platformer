using UnityEngine;

// Estado que al entrar reproduce desde cero el estado del Animator con su mismo nombre
public class AnimState : State
{
    protected string stateName;

    private readonly Animator animator;

    public AnimState(Animator animator, string stateName)
    {
        this.animator = animator;
        this.stateName = stateName;
    }

    public override void Enter()
    {
        animator.Play(stateName, 0, 0f);
    }

    public bool GetIsFinished()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName) && info.normalizedTime >= 1f;
    }
}
