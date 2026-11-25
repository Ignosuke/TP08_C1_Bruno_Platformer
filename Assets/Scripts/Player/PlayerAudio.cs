using UnityEngine;

// Reproduce los sonidos del PlayerSo cuando pasan los eventos. Si un clip no esta asignado no suena nada.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;
    [Tooltip("Pitch added at each combo step")]
    [SerializeField] private float attackPitchStep = 0.1f;
    [SerializeField] private float airJumpPitch = 1.25f;
    [SerializeField] private float landPitch = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float landVolume = 0.6f;
    [SerializeField] private float parryPitch = 1.3f;
    [Tooltip("The sword impact is the loudest clip")]
    [Range(0f, 1f)]
    [SerializeField] private float hitVolume = 0.4f;

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
        health.OnParried += PlayParry;
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
        health.OnParried -= PlayParry;
        healer.OnHealStarted -= PlayHeal;
    }

    private void PlayLand() => Play(playerData.GetLandClip(), landPitch, landVolume);
    private void PlayJump() => Play(playerData.GetJumpClip());
    private void PlayAirJump() => Play(playerData.GetAirJumpClip(), airJumpPitch);
    private void PlayDash() => Play(playerData.GetDashClip());
    private void PlayAttack(int step) => Play(playerData.GetAttackClips()[step - 1], 1f + (step - 1) * attackPitchStep);
    private void PlayAirAttack() => Play(playerData.GetAirAttackClip());
    /// <summary>El impacto de espada, mas bajo que el resto. Tambien lo usa el shuriken al pegar</summary>
    public void PlayHit() => Play(playerData.GetHitClip(), 1f, hitVolume);
    private void PlayDefend() => Play(playerData.GetDefendClip());
    private void PlayParry(DamageInfo info) => Play(playerData.GetDefendClip(), parryPitch);
    private void PlayHurt(DamageInfo info) => Play(playerData.GetHurtClip());
    private void PlayHeal() => Play(playerData.GetHealClip());

    // PlayOneShot permite que se pisen varios sonidos sin cortarse. Publico para los pickables, la tienda y el lanzador.
    // El pitch es de la fuente entera, por eso cada sonido lo vuelve a poner
    public void Play(AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        if (clip == null) return;
        // Warning: el pitch de la fuente tambien afecta a los PlayOneShot que siguen sonando. Si el tercer golpe (pitch 1.2)
        // todavia suena cuando llega el impacto (pitch 1), el tajo cambia de tono a mitad del clip. Usa una AudioSource por
        // tipo de sonido o deja el pitch fijo y varia solo el volumen.
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}
