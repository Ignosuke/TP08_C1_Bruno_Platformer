using System.Collections.Generic;
using UnityEngine;

// Ataca con los patrones del BossSo. Cada patron es una cadena de ataques que salen uno detras de otro.
// Elige al azar sin repetir el anterior. Empezado el patron los pasos siguientes salen desde mas lejos y cada tajo
// da un paso hacia el jugador, asi el empujon del golpe no corta la cadena. Solo se abandona con un aturdido,
// un golpe que lo corta, el cambio de fase o la muerte.
// El salto lo lanza hacia el jugador en el frame de compromiso y lo frena en el frame del golpe, el dibujo ya hace la altura.
// En la segunda fase siempre tiene armadura, los golpes le sacan vida pero no lo cortan
[RequireComponent(typeof(BossPhase))]
public class BossAttack : MeleeAttack
{
    private BossSo bossData;
    private BossPhase phase;
    private Rigidbody2D rb;
    private Animator animator;

    private readonly Queue<string> steps = new Queue<string>();
    private readonly List<int> options = new List<int>();
    private string currentStep;
    private int lastPattern = -1;
    private bool isChaining;            // El patron ya arranco, los pasos que quedan no se abandonan por distancia
    private bool isLunging;

    protected override void Awake()
    {
        base.Awake();
        bossData = (BossSo)enemyData;
        phase = GetComponent<BossPhase>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (bossData.GetPatternCount() == 0) Debug.LogError(name + " has no attack patterns in its BossSo", this);
    }

    protected override bool IsTargetInRange()
    {
        return sight.CanSeePlayer() && Mathf.Abs(sight.GetPlayerDistanceX()) <= GetNextRange();
    }

    // A que distancia sale el proximo paso, el salto de mas lejos y los de la cadena tambien. Si no hay patron elige uno
    public float GetNextRange()
    {
        FillSteps();
        if (steps.Peek() == bossData.GetLeapAttackName()) return bossData.GetLeapRange();
        return isChaining ? bossData.GetChainRange() : enemyData.GetAttackRange();
    }

    protected override EnemyAttackData PickAttack()
    {
        FillSteps();
        currentStep = steps.Dequeue();
        isChaining = true;
        isLunging = !GetIsLeaping();

        string stateName = phase.GetStateName(currentStep);
        EnemyAttackData attack = bossData.FindAttack(stateName);
        if (attack == null) Debug.LogError(name + " has no attack " + stateName + " in its BossSo", this);
        return attack;
    }

    // Si queda otro paso sigue sin espera, si no termina el patron y espera
    protected override void Finish()
    {
        isLunging = false;
        if (steps.Count > 0) EndAttack(0f);
        else Cancel();
    }

    // Cortado por un golpe, un parry, la muerte o el cambio de fase, el patron se abandona
    public override void Cancel()
    {
        steps.Clear();
        isChaining = false;
        isLunging = false;
        base.Cancel();
    }

    // El paso del tajo, en FixedUpdate como el movimiento. El mover esta apagado mientras ataca
    private void FixedUpdate()
    {
        if (!isLunging) return;

        int facing = mover.GetDirection();
        if (!GetIsAttacking() || sight.GetPlayerDistanceX() * facing <= bossData.GetLeapLandDistance())
        {
            isLunging = false;
            mover.Stop();
            return;
        }
        mover.Move(bossData.GetLungeSpeed() * phase.GetSpeedMultiplier());
    }

    public override void ResetAttack()
    {
        steps.Clear();
        lastPattern = -1;
        isChaining = false;
        isLunging = false;
        base.ResetAttack();
    }

    protected override float GetCooldown() => phase.GetPatternCooldown();

    public override bool GetIsArmored() => phase.GetIsPhaseTwo() || base.GetIsArmored();

    protected override void OnCommitFrame()
    {
        base.OnCommitFrame();
        if (GetIsAttacking() && GetIsLeaping()) Leap();
    }

    protected override void PerformHit()
    {
        if (GetIsLeaping() || isLunging) mover.Stop();   // Sin friccion seguiria deslizando
        isLunging = false;
        base.PerformHit();
    }

    // Recorre en el aire lo que falta hasta quedar frente al jugador, justo en el tiempo entre el compromiso y el golpe
    private void Leap()
    {
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length == 0) return;

        EnemyAttackData attack = GetCurrentAttack();
        float airTime = (attack.GetHitFrame() - attack.GetCommitFrame()) / clips[0].clip.frameRate;
        if (airTime <= 0f) return;

        int facing = mover.GetDirection();
        float distance = sight.GetPlayerDistanceX() - facing * bossData.GetLeapLandDistance();
        if (distance * facing < 0f) distance = 0f;   // Con el jugador encima no salta para atras
        distance = Mathf.Clamp(distance, -bossData.GetLeapRange(), bossData.GetLeapRange());

        rb.linearVelocity = new Vector2(distance / airTime, rb.linearVelocity.y);
    }

    private bool GetIsLeaping() => currentStep == bossData.GetLeapAttackName();

    // Si la cola quedo vacia elige el proximo patron, sin repetir el anterior si hay otro
    private void FillSteps()
    {
        if (steps.Count > 0) return;

        options.Clear();
        for (int i = 0; i < bossData.GetPatternCount(); i++)
        {
            BossPattern pattern = bossData.GetPattern(i);
            if (pattern.GetSteps().Count == 0 || (pattern.GetIsPhaseTwoOnly() && !phase.GetIsPhaseTwo())) continue;
            if (i != lastPattern) options.Add(i);
        }

        int picked = options.Count > 0 ? options[Random.Range(0, options.Count)] : lastPattern;
        if (picked < 0) return;

        lastPattern = picked;
        isChaining = false;     // El primer paso sale desde el alcance normal
        foreach (string step in bossData.GetPattern(picked).GetSteps()) steps.Enqueue(step);
    }
}
