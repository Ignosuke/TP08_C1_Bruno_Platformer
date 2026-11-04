using System;
using UnityEngine;

// Base de los ataques de los enemigos. Lleva el ciclo, arranca, pega en el frame del Animation Event, termina y espera.
// Los ataques del EnemySo se turnan en orden. Cada hijo decide cuando el jugador esta a tiro y que hace el golpe
[RequireComponent(typeof(EnemySight))]
public abstract class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected EnemySo enemyData;

    protected EnemySight sight;
    protected EnemyMover mover;
    private AnimationHitRelay hitRelay;

    private EnemyAttackData currentAttack;
    private int nextAttackIndex;
    private float attackCounter;
    private float cooldownCounter;
    private bool isAttacking;
    private bool hitDone;
    private bool isCommitted;           // Paso el punto sin retorno, los golpes ya no lo cortan

    public event Action OnAttackStarted;
    public event Action OnAttacked;     // En el frame del golpe, no cuando arranca
    public event Action OnParried;

    protected virtual void Awake()
    {
        sight = GetComponent<EnemySight>();
        mover = GetComponent<EnemyMover>();
        hitRelay = GetComponentInChildren<AnimationHitRelay>();   // En Visual, junto al Animator
    }

    private void OnEnable()
    {
        hitRelay.OnHitFrame += OnHitFrame;
        hitRelay.OnCommitFrame += OnCommitFrame;
    }

    private void OnDisable()
    {
        hitRelay.OnHitFrame -= OnHitFrame;
        hitRelay.OnCommitFrame -= OnCommitFrame;
    }

    // Si la animacion se corta el evento no llega, igual el ataque termina por tiempo
    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;
        if (!isAttacking) return;

        attackCounter -= Time.deltaTime;
        if (attackCounter <= 0f) Cancel();
    }

    protected abstract bool IsTargetInRange();

    protected abstract void PerformHit();

    public bool CanStart()
    {
        return !isAttacking && cooldownCounter <= 0f && IsTargetInRange();
    }

    public void StartAttack()
    {
        isAttacking = true;
        hitDone = false;
        isCommitted = false;
        currentAttack = enemyData.GetAttack(nextAttackIndex);
        nextAttackIndex = (nextAttackIndex + 1) % enemyData.GetAttackCount();
        attackCounter = currentAttack.GetDuration();
        OnAttackStarted?.Invoke();
    }

    // Termina o corta el ataque y arranca la espera hasta el proximo
    public void Cancel()
    {
        if (!isAttacking) return;
        isAttacking = false;
        isCommitted = false;
        cooldownCounter = enemyData.GetAttackCooldown();
    }

    public void ResetAttack()
    {
        isAttacking = false;
        isCommitted = false;
        cooldownCounter = 0f;
        nextAttackIndex = 0;
    }

    // Desde aca el golpe sale seguro, recibe dano pero no se corta, no se queja y no lo empujan
    private void OnCommitFrame()
    {
        if (isAttacking) isCommitted = true;
    }

    // Pega una sola vez y solo si el ataque sigue en curso
    private void OnHitFrame()
    {
        if (!isAttacking || hitDone) return;

        hitDone = true;
        OnAttacked?.Invoke();
        PerformHit();
    }

    // El jugador hizo parry perfecto, el ataque se corta y EnemyBrain lo aturde despues del choque
    protected void Parried()
    {
        Cancel();
        OnParried?.Invoke();
    }

    public EnemyAttackData GetCurrentAttack() => currentAttack;
    public bool GetIsAttacking() => isAttacking;
    public bool GetIsArmored() => isAttacking && isCommitted;
}
