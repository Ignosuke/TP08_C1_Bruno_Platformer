using UnityEngine;

// El grito del cambio de fase. Golpes, daño y muerte suenan con EnemyAudio y los clips del EnemySo
[RequireComponent(typeof(AudioSource), typeof(BossPhase))]
public class BossAudio : MonoBehaviour
{
    [SerializeField] private BossSo bossData;

    private AudioSource audioSource;
    private BossPhase phase;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        phase = GetComponent<BossPhase>();
    }

    private void OnEnable()
    {
        phase.OnShoutStarted += PlayShout;
    }

    private void OnDisable()
    {
        phase.OnShoutStarted -= PlayShout;
    }

    private void PlayShout()
    {
        AudioClip clip = bossData.GetShoutClip();
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
