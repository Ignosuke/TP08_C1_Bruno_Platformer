using UnityEngine;

// Reproduce los sonidos del EnemySo cuando pasan los eventos. Si un clip no esta asignado no suena nada
[RequireComponent(typeof(AudioSource), typeof(EnemyHealth))]
public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private EnemySo enemyData;

    private AudioSource audioSource;
    private EnemyHealth health;
    private EnemyAttack attack;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<EnemyHealth>();
        attack = GetComponent<EnemyAttack>();
        if (attack == null) Debug.LogError(name + " needs an EnemyAttack component", this);
    }

    private void OnEnable()
    {
        attack.OnHitFrame += PlayAttack;
        health.OnDamaged += PlayHurt;
        health.OnDied += PlayDeath;
    }

    private void OnDisable()
    {
        attack.OnHitFrame -= PlayAttack;
        health.OnDamaged -= PlayHurt;
        health.OnDied -= PlayDeath;
    }

    private void PlayAttack() => Play(enemyData.GetAttackClip());
    private void PlayDeath() => Play(enemyData.GetDeathClip());

    // El golpe que mata suena como muerte, no como daño
    private void PlayHurt(DamageInfo info)
    {
        if (!health.GetIsDead()) Play(enemyData.GetHurtClip());
    }

    // PlayOneShot permite que se pisen varios sonidos sin cortarse
    private void Play(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
