using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;

    private InputActionReference attackAction;

    private int[] comboDamage;
    private float[] comboDuration;
    private int airAttackDamage;
    private float airAttackDuration;
    private float attackHitDelay;
    private float comboWindow;
    private float attackBufferTime;
    private Vector2 hitboxOffset;
    private Vector2 hitboxSize;
    private LayerMask hitLayer;

    private float bufferCounter;
    private float attackCounter;
    private float hitCounter;
    private float comboCounter;

    private Rigidbody2D rb;
    private PlayerGroundChecker groundChecker;
    private PlayerMover mover;
    private PlayerDasher dasher;
    private PlayerDefender defender;
    private PlayerHealer healer;

    private bool attackRequested;
    private bool isAttacking;
    private bool isAirAttack;
    private bool hitChecked;
    private int comboStep;              // 0, 1 o 2 segun el golpe del combo
    private int currentDamage;

    public event Action<int> OnAttacked;   // Manda 1, 2 o 3
    public event Action OnAirAttacked;
    public event Action OnHit;

    private void Awake()
    {
        attackAction = playerData.GetAttackAction();
        comboDamage = playerData.GetComboDamage();
        comboDuration = playerData.GetComboDuration();
        airAttackDamage = playerData.GetAirAttackDamage();
        airAttackDuration = playerData.GetAirAttackDuration();
        attackHitDelay = playerData.GetAttackHitDelay();
        comboWindow = playerData.GetComboWindow();
        attackBufferTime = playerData.GetAttackBufferTime();
        hitboxOffset = playerData.GetHitboxOffset();
        hitboxSize = playerData.GetHitboxSize();
        hitLayer = playerData.GetHitLayer();

        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponentInChildren<PlayerGroundChecker>();
        mover = GetComponent<PlayerMover>();
        dasher = GetComponent<PlayerDasher>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
    }

    private void OnEnable()
    {
        attackAction.action.performed += OnAttackPerformed;
        attackAction.action.Enable();
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttackPerformed;
        attackAction.action.Disable();
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

        // El dash cancela el golpe
        if (isAttacking && dasher.GetIsDashing()) isAttacking = false;

        if (isAttacking) UpdateAttack();

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
            StartAttack();
    }

    private void UpdateAttack()
    {
        hitCounter -= Time.deltaTime;
        attackCounter -= Time.deltaTime;

        // El golpe pega una sola vez, un ratito despues de empezar
        if (!hitChecked && hitCounter <= 0f)
        {
            hitChecked = true;
            CheckHitbox();
        }

        if (attackCounter <= 0f) EndAttack();
    }

    private void StartAttack()
    {
        bufferCounter = 0f;
        isAttacking = true;
        hitChecked = false;
        hitCounter = attackHitDelay;

        if (!groundChecker.GetIsGrounded())
        {
            isAirAttack = true;
            attackCounter = airAttackDuration;
            currentDamage = airAttackDamage;
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
        attackCounter = comboDuration[comboStep];
        currentDamage = comboDamage[comboStep];
        OnAttacked?.Invoke(comboStep + 1);
    }

    private void EndAttack()
    {
        isAttacking = false;

        // Despues del ultimo golpe o de un ataque aereo el combo arranca de nuevo
        bool isLastStep = comboStep >= comboDamage.Length - 1;
        comboCounter = (isLastStep || isAirAttack) ? 0f : comboWindow;
    }

    private void CheckHitbox()
    {
        int facing = mover.GetFacing();
        Collider2D[] hits = Physics2D.OverlapBoxAll(GetHitboxCenter(), hitboxSize, 0f, hitLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.attachedRigidbody == rb) continue;   // No pegarse a uno mismo

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            damageable.TakeDamage(currentDamage, new Vector2(facing, 0f));
            OnHit?.Invoke();
        }
    }

    public Vector2 GetHitboxCenter()
    {
        return (Vector2)transform.position + new Vector2(hitboxOffset.x * mover.GetFacing(), hitboxOffset.y);
    }

    public Vector2 GetHitboxSize()
    {
        return hitboxSize;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    // Termino el golpe pero todavia se puede encadenar el siguiente
    public bool GetIsComboWindowOpen()
    {
        return !isAttacking && comboCounter > 0f;
    }

    // La hitbox depende de hacia donde mira el mover, por eso solo se dibuja en Play.
    // Si se selecciona el prefab en Play, esa copia no corrio Awake y el mover es null
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || mover == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetHitboxCenter(), GetHitboxSize());
    }
}
