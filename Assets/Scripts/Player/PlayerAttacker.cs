using System;
using UnityEngine;
using UnityEngine.InputSystem;

// El combo de tres golpes en el piso y el ataque aereo. El golpe pega cuando la animacion llega a su frame,
// con el Animation Event que el builder pone en cada clip de ataque
public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private const string CounterClipName = "PlayerAttack1";    // El clip que arma PlayerAnimatorBuilder para Attack1

    private InputActionReference attackAction;

    private int[] comboDamage;
    private float[] comboDuration;
    private int airAttackDamage;
    private float airAttackDuration;
    private float comboWindow;
    private float attackBufferTime;
    private Vector2 hitboxOffset;
    private Vector2 hitboxSize;
    private LayerMask hitLayer;
    private float[] comboKnockback;
    private float[] comboHitStop;
    private float airAttackKnockback;
    private float airAttackHitStop;
    private float parryHitStop;
    private float counterStartTime;     // Fraccion de Attack1 donde arranca el contraataque

    private float bufferCounter;
    private float attackCounter;
    private float comboCounter;

    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerDasher dasher;
    private PlayerDefender defender;
    private PlayerHealer healer;
    private PlayerHealth health;
    private MeleeHitbox hitbox;
    private AnimationHitRelay hitRelay;

    private bool attackRequested;
    private bool isAttacking;
    private bool isAirAttack;
    private bool hitChecked;
    private int comboStep;              // 0, 1 o 2 segun el golpe del combo
    private int currentDamage;
    private float currentKnockback;
    private float currentHitStop;
    private float startTime;            // Fraccion del clip donde arranco este golpe, 0 salvo en el contraataque

    public event Action<int> OnAttacked;   // Manda 1, 2 o 3
    public event Action OnAirAttacked;

    private void Awake()
    {
        attackAction = playerData.GetAttackAction();
        comboDamage = playerData.GetComboDamage();
        comboDuration = playerData.GetComboDuration();
        airAttackDamage = playerData.GetAirAttackDamage();
        airAttackDuration = playerData.GetAirAttackDuration();
        comboWindow = playerData.GetComboWindow();
        attackBufferTime = playerData.GetAttackBufferTime();
        hitboxOffset = playerData.GetHitboxOffset();
        hitboxSize = playerData.GetHitboxSize();
        hitLayer = playerData.GetHitLayer();
        comboKnockback = playerData.GetComboKnockback();
        comboHitStop = playerData.GetComboHitStop();
        airAttackKnockback = playerData.GetAirAttackKnockback();
        airAttackHitStop = playerData.GetAirAttackHitStop();

        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        mover = GetComponent<PlayerMover>();
        dasher = GetComponent<PlayerDasher>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
        health = GetComponent<PlayerHealth>();
        hitbox = GetComponent<MeleeHitbox>();
        hitRelay = GetComponentInChildren<AnimationHitRelay>();   // En Visual, junto al Animator

        parryHitStop = playerData.GetParryHitStop();
        counterStartTime = FindCounterStartTime(hitRelay.GetComponent<Animator>(), playerData.GetCounterStartFrame());
    }

    // Pasa el frame del PlayerSo a fraccion del clip. Los frames salen del clip porque el builder pone fps = frames / duracion.
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
        attackAction.action.performed += OnAttackPerformed;
        attackAction.action.Enable();
        hitRelay.OnHitFrame += CheckHitbox;
        groundChecker.OnLanded += EndAirAttack;
        health.OnParried += Counter;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttackPerformed;
        attackAction.action.Disable();
        hitRelay.OnHitFrame -= CheckHitbox;
        groundChecker.OnLanded -= EndAirAttack;
        health.OnParried -= Counter;
        isAttacking = false;
        attackRequested = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        attackRequested = true;
    }

    private void Update()
    {
        if (comboCounter > 0f) comboCounter -= Time.deltaTime;

        // El dash cancela el golpe, la animacion cambia y el evento del golpe ya no llega
        if (isAttacking && dasher.GetIsDashing()) isAttacking = false;

        if (isAttacking)
        {
            attackCounter -= Time.deltaTime;
            if (attackCounter <= 0f) EndAttack();
        }

        // Igual que el salto, el buffer guarda el click para encadenar el siguiente golpe al terminar este
        if (attackRequested)
        {
            attackRequested = false;
            bufferCounter = attackBufferTime;
        }
        else if (bufferCounter > 0f)
        {
            bufferCounter -= Time.deltaTime;
        }

        if (bufferCounter > 0f && !isAttacking && !dasher.GetIsDashing() && !defender.GetIsDefending() && !healer.GetIsHealing())
            StartAttack(0f);
    }

    // groundStartTime es la fraccion del clip donde arranca, el golpe dura solo lo que le queda al clip
    private void StartAttack(float groundStartTime)
    {
        bufferCounter = 0f;
        isAttacking = true;
        hitChecked = false;
        startTime = 0f;

        if (!groundChecker.GetIsGrounded())
        {
            isAirAttack = true;
            attackCounter = airAttackDuration;
            currentDamage = airAttackDamage;
            currentKnockback = airAttackKnockback;
            currentHitStop = airAttackHitStop;
            OnAirAttacked?.Invoke();
            return;
        }

        isAirAttack = false;

        // Si todavia estamos dentro de la ventana del combo sigue el proximo golpe, si no vuelve al primero
        if (comboCounter > 0f)
            comboStep++;
        else
            comboStep = 0;

        comboCounter = 0f;
        startTime = groundStartTime;
        attackCounter = comboDuration[comboStep] * (1f - startTime);
        currentDamage = comboDamage[comboStep];
        currentKnockback = comboKnockback[comboStep];
        currentHitStop = comboHitStop[comboStep];
        OnAttacked?.Invoke(comboStep + 1);
    }

    // Parry perfecto a un golpe cuerpo a cuerpo. Congela el choque y se ve el primer golpe del combo desde el tajo, pero no pega.
    // El aturdido lo pone el enemigo. Ya mira al atacante porque el parry solo vale de frente
    private void Counter(DamageInfo info)
    {
        if (!info.IsMelee) return;

        hitbox.RaiseHitStop(parryHitStop);
        defender.StopDefend();
        comboCounter = 0f;      // Sin ventana de combo arranca en Attack1
        StartAttack(counterStartTime);
        hitChecked = true;      // Asi el HitFrame de este golpe no hace nada
    }

    private void EndAttack()
    {
        isAttacking = false;

        // Despues del ultimo golpe o de un ataque aereo el combo arranca de nuevo
        bool isLastStep = comboStep >= comboDamage.Length - 1;
        comboCounter = (isLastStep || isAirAttack) ? 0f : comboWindow;
    }

    // Al tocar el piso el ataque aereo termina enseguida, asi PlayerAnimator vuelve a Idle o Run
    private void EndAirAttack()
    {
        if (isAttacking && isAirAttack) EndAttack();
    }

    // Lo llama el Animation Event. Pega una sola vez y solo si el golpe sigue en curso
    private void CheckHitbox()
    {
        if (!isAttacking || hitChecked) return;

        hitChecked = true;
        hitbox.Hit(currentDamage, currentKnockback, hitboxOffset, hitboxSize, hitLayer, mover.GetFacing(), currentHitStop);
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public float GetStartTime() => startTime;

    // Termino el golpe pero todavia se puede encadenar el siguiente
    public bool GetIsComboWindowOpen()
    {
        return !isAttacking && comboCounter > 0f;
    }
}
