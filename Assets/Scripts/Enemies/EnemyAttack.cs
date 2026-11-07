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
    public event Action OnHitFrame;     // En el frame del golpe, pegue o no, no cuando arranca
    public event Action OnParried;

    protected virtual void Awake()
    {
        sight = GetComponent<EnemySight>();
        mover = GetComponent<EnemyMover>();
        hitRelay = GetComponentInChildren<AnimationHitRelay>();   // En Visual, junto al Animator
    }

    private void OnEnable()
    {
        hitRelay.OnHitFrame += CheckHit;
        hitRelay.OnCommitFrame += OnCommitFrame;
    }

    private void OnDisable()
    {
        hitRelay.OnHitFrame -= CheckHit;
        hitRelay.OnCommitFrame -= OnCommitFrame;
    }

    // Si la animacion se corta el evento no llega, igual el ataque termina por tiempo
    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;
        if (!isAttacking) return;

        attackCounter -= Time.deltaTime;
        if (attackCounter <= 0f) Finish();
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
        currentAttack = PickAttack();
        attackCounter = currentAttack.GetDuration();
        OnAttackStarted?.Invoke();
    }

    // Por defecto los ataques del EnemySo se turnan en orden
    protected virtual EnemyAttackData PickAttack()
    {
        EnemyAttackData attack = enemyData.GetAttack(nextAttackIndex);
        nextAttackIndex = (nextAttackIndex + 1) % enemyData.GetAttackCount();
        return attack;
    }

    // El ataque llego al final, por defecto es lo mismo que cortarlo
    protected virtual void Finish() => Cancel();

    // Termina o corta el ataque y arranca la espera hasta el proximo
    public virtual void Cancel()
    {
        if (!isAttacking) return;
        EndAttack(GetCooldown());
    }

    protected void EndAttack(float cooldown)
    {
        isAttacking = false;
        isCommitted = false;
        cooldownCounter = cooldown;
    }

    protected virtual float GetCooldown() => enemyData.GetAttackCooldown();

    public virtual void ResetAttack()
    {
        isAttacking = false;
        isCommitted = false;
        cooldownCounter = 0f;
        nextAttackIndex = 0;
    }

    // Desde aca el golpe sale seguro, recibe daño pero no se corta, no se queja y no lo empujan
    protected virtual void OnCommitFrame()
    {
        if (isAttacking) isCommitted = true;
    }

    // Pega una sola vez y solo si el ataque sigue en curso
    private void CheckHit()
    {
        if (!isAttacking || hitDone) return;

        hitDone = true;
        OnHitFrame?.Invoke();
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
    public virtual bool GetIsArmored() => isAttacking && isCommitted;
}
