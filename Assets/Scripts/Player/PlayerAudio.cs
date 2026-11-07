using UnityEngine;

// Reproduce los sonidos del PlayerSo cuando pasan los eventos. Si un clip no esta asignado no suena nada.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private AudioSource audioSource;
    private PlayerGroundChecker groundChecker;
    private PlayerJumper jumper;
    private PlayerDasher dasher;
    private PlayerAttacker attacker;
    private PlayerDefender defender;
    private PlayerHealth health;
    private PlayerHealer healer;
    private MeleeHitbox hitbox;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        jumper = GetComponent<PlayerJumper>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
        health = GetComponent<PlayerHealth>();
        healer = GetComponent<PlayerHealer>();
        hitbox = GetComponent<MeleeHitbox>();
    }

    private void OnEnable()
    {
        groundChecker.OnLanded += PlayLand;
        jumper.OnJumped += PlayJump;
        jumper.OnAirJumped += PlayAirJump;
        dasher.OnDashed += PlayDash;
        attacker.OnAttacked += PlayAttack;
        attacker.OnAirAttacked += PlayAirAttack;
        hitbox.OnHit += PlayHit;
        defender.OnDefendStarted += PlayDefend;
        health.OnDamaged += PlayHurt;
        healer.OnHealStarted += PlayHeal;
    }

    private void OnDisable()
    {
        groundChecker.OnLanded -= PlayLand;
        jumper.OnJumped -= PlayJump;
        jumper.OnAirJumped -= PlayAirJump;
        dasher.OnDashed -= PlayDash;
        attacker.OnAttacked -= PlayAttack;
        attacker.OnAirAttacked -= PlayAirAttack;
        hitbox.OnHit -= PlayHit;
        defender.OnDefendStarted -= PlayDefend;
        health.OnDamaged -= PlayHurt;
        healer.OnHealStarted -= PlayHeal;
    }

    private void PlayLand() => Play(playerData.GetLandClip());
    private void PlayJump() => Play(playerData.GetJumpClip());
    private void PlayAirJump() => Play(playerData.GetAirJumpClip());
    private void PlayDash() => Play(playerData.GetDashClip());
    private void PlayAttack(int step) => Play(playerData.GetAttackClips()[step - 1]);
    private void PlayAirAttack() => Play(playerData.GetAirAttackClip());
    private void PlayHit() => Play(playerData.GetHitClip());
    private void PlayDefend() => Play(playerData.GetDefendClip());
    private void PlayHurt(DamageInfo info) => Play(playerData.GetHurtClip());
    private void PlayHeal() => Play(playerData.GetHealClip());

    // PlayOneShot permite que se pisen varios sonidos sin cortarse
    private void Play(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
