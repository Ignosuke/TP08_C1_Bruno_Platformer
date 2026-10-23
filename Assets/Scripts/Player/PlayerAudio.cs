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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        jumper = GetComponent<PlayerJumper>();
        dasher = GetComponent<PlayerDasher>();
        attacker = GetComponent<PlayerAttacker>();
        defender = GetComponent<PlayerDefender>();
    }

    private void OnEnable()
    {
        groundChecker.OnLanded += PlayLand;
        jumper.OnJumped += PlayJump;
        dasher.OnDashed += PlayDash;
        attacker.OnAttacked += PlayAttack;
        attacker.OnAirAttacked += PlayAirAttack;
        attacker.OnHit += PlayHit;
        defender.OnDefendStarted += PlayDefend;
    }

    private void OnDisable()
    {
        groundChecker.OnLanded -= PlayLand;
        jumper.OnJumped -= PlayJump;
        dasher.OnDashed -= PlayDash;
        attacker.OnAttacked -= PlayAttack;
        attacker.OnAirAttacked -= PlayAirAttack;
        attacker.OnHit -= PlayHit;
        defender.OnDefendStarted -= PlayDefend;
    }

    private void PlayLand() => Play(playerData.GetLandClip());
    private void PlayJump() => Play(playerData.GetJumpClip());
    private void PlayDash() => Play(playerData.GetDashClip());
    private void PlayAttack(int step) => Play(playerData.GetAttackClips()[step - 1]);
    private void PlayAirAttack() => Play(playerData.GetAirAttackClip());
    private void PlayHit() => Play(playerData.GetHitClip());
    private void PlayDefend() => Play(playerData.GetDefendClip());

    // PlayOneShot permite que se pisen varios sonidos sin cortarse
    private void Play(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
