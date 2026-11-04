using System;
using UnityEngine;

// Los valores de un ataque del enemigo. El EnemySo tiene una lista y los ataques se turnan en ese orden
[Serializable]
public class EnemyAttackData
{
    [Tooltip("Animator state and clip suffix of this attack, e.g. Attack or Attack1")]
    [SerializeField] private string stateName = "Attack";
    [SerializeField] private int damage = 2;                    // En medios corazones
    [Tooltip("Sheet frame (1-based) where the attack lands. The builder turns it into an Animation Event")]
    [SerializeField] private int hitFrame = 10;
    [Tooltip("Sheet frame (1-based) from which player hits no longer interrupt the attack. 0 = never armored")]
    [SerializeField] private int commitFrame = 6;
    [SerializeField] private float duration = 1.2f;             // Lo que dura la animacion de este ataque
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1.4f, 0.5f);   // Mirando a la derecha, desde los pies
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.8f, 1f);
    [Tooltip("Push given to the player by this attack")]
    [SerializeField] private float knockback = 8f;
    [Tooltip("Game freeze in real seconds when the attack lands, 0 = none")]
    [SerializeField] private float hitStop = 0.08f;
    [Tooltip("Camera shake strength in world units on the hit frame, lands or not. 0 = none")]
    [SerializeField] private float shakeMagnitude = 0.15f;
    [SerializeField] private float shakeDuration = 0.25f;

    // Unity lo necesita para que un ataque nuevo en el inspector arranque con los valores de arriba
    public EnemyAttackData() { }

    // Lo usan los builders para cargar los valores iniciales
    public EnemyAttackData(string stateName, int damage, int hitFrame, int commitFrame, float duration, Vector2 hitboxOffset,
        Vector2 hitboxSize, float knockback, float hitStop, float shakeMagnitude, float shakeDuration)
    {
        this.stateName = stateName;
        this.damage = damage;
        this.hitFrame = hitFrame;
        this.commitFrame = commitFrame;
        this.duration = duration;
        this.hitboxOffset = hitboxOffset;
        this.hitboxSize = hitboxSize;
        this.knockback = knockback;
        this.hitStop = hitStop;
        this.shakeMagnitude = shakeMagnitude;
        this.shakeDuration = shakeDuration;
    }

    public string GetStateName() => stateName;
    public int GetDamage() => damage;
    public int GetHitFrame() => hitFrame;
    public int GetCommitFrame() => commitFrame;
    public float GetDuration() => duration;
    public Vector2 GetHitboxOffset() => hitboxOffset;
    public Vector2 GetHitboxSize() => hitboxSize;
    public float GetKnockback() => knockback;
    public float GetHitStop() => hitStop;
    public float GetShakeMagnitude() => shakeMagnitude;
    public float GetShakeDuration() => shakeDuration;
}
