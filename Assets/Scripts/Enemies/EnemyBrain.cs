using System;
using UnityEngine;

// Coordina al enemigo. Arranca el ataque cuando puede, lo aturde si le hacen parry (despues del choque),
// lo da vuelta hacia quien le pega y lo devuelve a su lugar cuando el jugador reaparece
[RequireComponent(typeof(EnemyHealth), typeof(EnemySight))]
public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private EnemySo enemyData;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemySight sight;
    private EnemyMover mover;
    private EnemyAttack attack;
    private PlayerHealth player;

    private Vector3 spawnPosition;
    private float stunCounter;
    private float recoilCounter;        // Choque del parry, sostiene el golpe antes de aturdirse

    public event Action OnStunned;                     // Para el indicador de aturdido (estrellitas)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        sight = GetComponent<EnemySight>();
        mover = GetComponent<EnemyMover>();
        attack = GetComponent<EnemyAttack>();
        if (mover == null) Debug.LogError(name + " needs an EnemyMover component", this);
        if (attack == null) Debug.LogError(name + " needs an EnemyAttack component", this);
        spawnPosition = transform.position;

        // En Awake y no en OnEnable porque muerto queda apagado y tiene que escuchar igual
        player = sight.GetPlayer();
        if (player != null) player.OnRespawned += ResetToSpawn;
    }

    private void OnDestroy()
    {
        if (player != null) player.OnRespawned -= ResetToSpawn;
    }

    private void OnEnable()
    {
        health.OnDamaged += FaceAttacker;
        health.OnDied += attack.Cancel;
        attack.OnParried += Stun;
    }

    private void OnDisable()
    {
        health.OnDamaged -= FaceAttacker;
        health.OnDied -= attack.Cancel;
        attack.OnParried -= Stun;
    }

    private void Update()
    {
        if (stunCounter > 0f) stunCounter -= Time.deltaTime;
        if (recoilCounter > 0f)
        {
            recoilCounter -= Time.deltaTime;
            if (recoilCounter <= 0f) StartStun();
        }

        bool isBusy = health.GetIsDead() || health.GetIsHurt() || GetIsStunned() || GetIsRecoiling();
        if (!isBusy && attack.CanStart())
        {
            mover.FaceTowards(sight.GetPlayerDistanceX());
            attack.StartAttack();
        }

        mover.enabled = !isBusy && !attack.GetIsAttacking();
    }

    // El golpe corta el ataque y lo da vuelta hacia el atacante, salvo despues del punto sin retorno
    private void FaceAttacker(DamageInfo info)
    {
        if (attack.GetIsArmored()) return;

        attack.Cancel();
        mover.SetDirection(info.Direction.x > 0f ? -1 : 1);
    }

    // En tiempo escalado, asi espera al hitstop del parry. Con 0 se aturde enseguida
    private void Stun()
    {
        recoilCounter = enemyData.GetParryRecoilTime();
        if (recoilCounter <= 0f) StartStun();
    }

    // El aturdido se cuenta desde aca. Muerto no se aturde
    private void StartStun()
    {
        recoilCounter = 0f;
        if (health.GetIsDead()) return;

        stunCounter = enemyData.GetStunDuration();
        OnStunned?.Invoke();
    }

    // Apagar y prender reinicia a todos los componentes, barra escondida, contacto prendido y animacion de cero
    public void ResetToSpawn()
    {
        gameObject.SetActive(false);

        transform.position = spawnPosition;
        stunCounter = 0f;
        recoilCounter = 0f;
        health.ResetHealth();
        attack.ResetAttack();
        mover.ResetMover();

        gameObject.SetActive(true);
        rb.linearVelocity = Vector2.zero;
    }

    public bool GetIsStunned() => stunCounter > 0f;
    public bool GetIsRecoiling() => recoilCounter > 0f;
}
