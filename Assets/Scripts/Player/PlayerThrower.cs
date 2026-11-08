using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

// Lanza shurikens con Abajo + Ataque (lo decide PlayerAttacker) o con el gatillo derecho del joystick.
// El shuriken sale en el frame de la animacion Throw, con el mismo Animation Event que los golpes.
// La municion se queda al morir, como las monedas, y se pierde cuando termina la partida
[RequireComponent(typeof(PlayerMover), typeof(PlayerDasher), typeof(PlayerDefender))]
[RequireComponent(typeof(PlayerHealer), typeof(PlayerAttacker))]
public class PlayerThrower : MonoBehaviour
{
    [SerializeField] private PlayerSo playerData;
    [SerializeField] private Shuriken shurikenPrefab;

    private InputActionReference moveAction;
    private InputAction triggerAction;      // Se arma aca porque el asset de input no la tiene

    private float throwDuration;
    private float throwCooldown;
    private float downThreshold;
    private Vector2 spawnOffset;

    private float throwCounter;
    private float cooldownCounter;

    private PlayerMover mover;
    private PlayerDasher dasher;
    private PlayerDefender defender;
    private PlayerHealer healer;
    private PlayerAttacker attacker;
    private AnimationHitRelay hitRelay;
    private PlayerAudio playerAudio;
    private ObjectPool<Shuriken> pool;

    private int ammo;
    private bool isThrowing;
    private bool hasSpawned;
    private bool triggerRequested;

    public event Action<int> OnAmmoChanged;   // Manda la municion actual

    private void Awake()
    {
        moveAction = playerData.GetMoveAction();
        throwDuration = playerData.GetThrowDuration();
        throwCooldown = playerData.GetThrowCooldown();
        downThreshold = playerData.GetThrowDownThreshold();
        spawnOffset = playerData.GetShurikenSpawnOffset();
        ammo = playerData.GetStartShurikens();

        mover = GetComponent<PlayerMover>();
        dasher = GetComponent<PlayerDasher>();
        defender = GetComponent<PlayerDefender>();
        healer = GetComponent<PlayerHealer>();
        attacker = GetComponent<PlayerAttacker>();
        playerAudio = GetComponent<PlayerAudio>();
        hitRelay = GetComponentInChildren<AnimationHitRelay>();   // En Visual, junto al Animator

        triggerAction = new InputAction("Throw", InputActionType.Button);
        triggerAction.AddBinding("<Gamepad>/rightTrigger");

        pool = new ObjectPool<Shuriken>(CreateShuriken, GetShuriken, ReleaseShuriken, DestroyShuriken);
    }

    private void OnEnable()
    {
        triggerAction.performed += OnTriggerPerformed;
        triggerAction.Enable();
        hitRelay.OnHitFrame += SpawnShuriken;
    }

    private void OnDisable()
    {
        triggerAction.performed -= OnTriggerPerformed;
        triggerAction.Disable();
        hitRelay.OnHitFrame -= SpawnShuriken;
        isThrowing = false;
        triggerRequested = false;
    }

    // Los shurikens sueltos no son hijos del jugador, los guardados se destruyen con el
    private void OnDestroy()
    {
        triggerAction.Dispose();
        pool.Clear();
        pool = null;
    }

    private void OnTriggerPerformed(InputAction.CallbackContext context)
    {
        triggerRequested = true;
    }

    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;

        if (isThrowing)
        {
            // El dash o un golpe recibido (apagan al atacante) cortan el lanzamiento antes de soltar el shuriken
            throwCounter -= Time.deltaTime;
            if (throwCounter <= 0f || dasher.GetIsDashing() || !attacker.enabled) isThrowing = false;
        }

        if (triggerRequested)
        {
            triggerRequested = false;
            TryThrow();
        }
    }

    private bool CanThrow()
    {
        return ammo > 0 && !isThrowing && cooldownCounter <= 0f && attacker.enabled && !attacker.GetIsAttacking()
            && !dasher.GetIsDashing() && !defender.GetIsDefending() && !healer.GetIsHealing();
    }

    // Arranca la animacion, la municion se gasta al soltar el shuriken. Devuelve false si no pudo
    public bool TryThrow()
    {
        if (!CanThrow()) return false;

        isThrowing = true;
        hasSpawned = false;
        throwCounter = throwDuration;
        cooldownCounter = throwCooldown;
        return true;
    }

    // Lo llama el Animation Event, sale una sola vez y solo si el lanzamiento sigue en curso
    private void SpawnShuriken()
    {
        if (!isThrowing || hasSpawned || ammo <= 0) return;

        hasSpawned = true;
        ammo--;
        OnAmmoChanged?.Invoke(ammo);

        int facing = mover.GetFacing();
        Vector3 spawn = transform.position + new Vector3(spawnOffset.x * facing, spawnOffset.y, 0f);
        playerAudio.Play(playerData.GetThrowClip());
        Shuriken shuriken = pool.Get();
        shuriken.Launch(playerData, facing, spawn, new Vector2(transform.position.x, spawn.y), ReturnShuriken, PlayShurikenHit);
    }

    // La compra de la tienda
    public void AddAmmo(int amount)
    {
        ammo += amount;
        OnAmmoChanged?.Invoke(ammo);
    }

    private void PlayShurikenHit() => playerAudio.PlayHit();

    // Si el jugador ya no existe el shuriken que seguia volando se destruye solo
    private void ReturnShuriken(Shuriken shuriken)
    {
        if (pool != null) pool.Release(shuriken);
        else Destroy(shuriken.gameObject);
    }

    private Shuriken CreateShuriken() => Instantiate(shurikenPrefab);
    private static void GetShuriken(Shuriken shuriken) => shuriken.gameObject.SetActive(true);
    private static void ReleaseShuriken(Shuriken shuriken) => shuriken.gameObject.SetActive(false);

    private static void DestroyShuriken(Shuriken shuriken)
    {
        if (shuriken != null) Destroy(shuriken.gameObject);
    }

    // Abajo + ataque cuenta como lanzar solo si queda municion, si no es un golpe normal
    public bool GetWantsThrow() => ammo > 0 && moveAction.action.ReadValue<Vector2>().y < -downThreshold;

    public bool GetIsThrowing() => isThrowing;
    public int GetAmmo() => ammo;
}
