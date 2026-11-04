using System;
using UnityEngine;

// Va en el mismo objeto que el Animator. El Animation Event del frame del golpe llama a HitFrame
// y este lo pasa al atacante (PlayerAttacker o EnemyAttack), que estan en la raiz.
// Los enemigos tienen otro evento antes, CommitFrame, desde ahi el ataque ya no se corta
public class AnimationHitRelay : MonoBehaviour
{
    public event Action OnHitFrame;
    public event Action OnCommitFrame;

    // Lo llama el Animation Event, el nombre tiene que coincidir con el de los builders
    public void HitFrame()
    {
        OnHitFrame?.Invoke();
    }

    // Lo llama el Animation Event del punto sin retorno, el nombre tiene que coincidir con el de los builders
    public void CommitFrame()
    {
        OnCommitFrame?.Invoke();
    }
}
