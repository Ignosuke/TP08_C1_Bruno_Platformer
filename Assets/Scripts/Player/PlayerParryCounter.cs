using UnityEngine;

// Parry perfecto a un golpe cuerpo a cuerpo. Congela el choque y se ve el primer golpe del combo desde el tajo, pero no pega.
// El aturdido lo pone el enemigo. Ya mira al atacante porque el parry solo vale de frente
[RequireComponent(typeof(PlayerHealth), typeof(PlayerAttacker), typeof(PlayerDefender))]
[RequireComponent(typeof(MeleeHitbox))]
public class PlayerParryCounter : MonoBehaviour
{
    private const string CounterClipName = "PlayerAttack1";    // El clip del estado Attack1

    [SerializeField] private PlayerSo playerData;
    [SerializeField] private Animator animator;

    private float parryHitStop;
    private float counterStartTime;     // Fraccion de Attack1 donde arranca el contraataque

    private PlayerHealth health;
    private PlayerAttacker attacker;
    private PlayerDefender defender;
    private MeleeHitbox hitbox;

    private void Awake()
    {
        parryHitStop = playerData.GetParryHitStop();
        counterStartTime = FindCounterStartTime(animator, playerData.GetCounterStartFrame());

        health = GetComponent<PlayerHealth>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
        hitbox = GetComponent<MeleeHitbox>();
    }

    // Pasa el frame del PlayerSo a fraccion del clip, contando los frames con el fps del clip.
    // Sin Animator o sin el clip arranca desde el principio
    private static float FindCounterStartTime(Animator animator, int frame)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return 0f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name != CounterClipName) continue;

            float frameCount = Mathf.Round(clip.length * clip.frameRate);
            if (frameCount <= 0f) return 0f;
            return Mathf.Clamp(frame - 1, 0f, frameCount - 1f) / frameCount;
        }
        return 0f;
    }

    private void OnEnable()
    {
        health.OnParried += Counter;
    }

    private void OnDisable()
    {
        health.OnParried -= Counter;
    }

    // Con el ataque apagado, en el empujon o muerto, no hay contraataque
    private void Counter(DamageInfo info)
    {
        if (!info.IsMelee || !attacker.enabled) return;

        hitbox.RaiseHitStop(parryHitStop);
        defender.StopDefend();
        attacker.StartCounter(counterStartTime);
    }
}
