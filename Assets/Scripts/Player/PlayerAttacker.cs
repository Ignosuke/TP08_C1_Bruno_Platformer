using System;
using UnityEngine;
using UnityEngine.InputSystem;

// El combo de tres golpes en el piso y el ataque aereo. El golpe pega cuando la animacion llega a su frame,
// con el Animation Event de cada clip de ataque
[RequireComponent(typeof(PlayerMover), typeof(PlayerDasher), typeof(PlayerDefender))]
[RequireComponent(typeof(PlayerHealer), typeof(MeleeHitbox))]
public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

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

    private float bufferCounter;
    private float attackCounter;
    private float comboCounter;

    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerDasher dasher;
    private PlayerDefender defender;
    private PlayerHealer healer;
    private MeleeHitbox hitbox;
    private AnimationHitRelay hitRelay;

    private bool attackRequested;
    private bool isAttacking;
    private bool isAirAttack;
    private bool hitChecked;
    private int comboStep;              // 0, 1 o 2 segun el golpe del combo
    private int currentDamage;
    private int bonusDamage;            // Del pickable de daño, se suma a todos los golpes
    private float currentKnockback;
    private float currentHitStop;
    private float startTime;            // Fraccion del clip donde arranco este golpe, 0 salvo en el contraataque

    public event Action<int> OnAttacked;   // Manda 1, 2 o 3
    public event Action OnAirAttacked;

    private void Awake()
    {
        attackAction = playerData.GetAttackAction();
        comboDamage = (int[])playerData.GetComboDamage().Clone();   // Copia, el afilado de la tienda no tiene que tocar el asset
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
        hitbox = GetComponent<MeleeHitbox>();
        hitRelay = GetComponentInChildren<AnimationHitRelay>();   // En Visual, junto al Animator
    }

    private void OnEnable()
    {
        attackAction.action.performed += OnAttackPerformed;
        attackAction.action.Enable();
        hitRelay.OnHitFrame += CheckHitbox;
        groundChecker.OnLanded += EndAirAttack;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttackPerformed;
        attackAction.action.Disable();
        hitRelay.OnHitFrame -= CheckHitbox;
        groundChecker.OnLanded -= EndAirAttack;
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

    // El contraataque del parry, lo llama PlayerParryCounter. groundStartTime es la fraccion de Attack1 donde arranca
    public void StartCounter(float groundStartTime)
    {
        comboCounter = 0f;      // Sin ventana de combo arranca en Attack1
        StartAttack(groundStartTime);
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
        hitbox.Hit(currentDamage + bonusDamage, currentKnockback, hitboxOffset, hitboxSize, hitLayer, mover.GetFacing(), currentHitStop);
    }

    // El afilado de la tienda, step es 0, 1 o 2 segun el golpe del combo
    public void AddComboDamage(int step, int amount)
    {
        if (step < comboDamage.Length) comboDamage[step] += amount;
    }

    public void SetBonusDamage(int amount)
    {
        bonusDamage = amount;
    }

    public bool GetIsAttacking() => isAttacking;

    public float GetStartTime() => startTime;

    // Termino el golpe pero todavia se puede encadenar el siguiente
    public bool GetIsComboWindowOpen() => !isAttacking && comboCounter > 0f;
}
