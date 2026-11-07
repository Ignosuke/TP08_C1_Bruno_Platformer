using System;
using UnityEngine;

// La fase del jefe y los valores que cambian con ella. La primera usa los del EnemySo y la segunda los del BossSo.
// BossPhaseCutscene la hace avanzar, el resto solo pregunta
public class BossPhase : MonoBehaviour
{
    public const string FlamingSuffix = "Flaming";     // Los estados de la segunda fase se llaman como los de la primera mas esto

    [SerializeField] private BossSo bossData;

    private int phase = 1;
    private bool isChanging;            // Toda la escena del cambio, no recibe daño
    private bool isShouting;
    private bool isRecovering;          // Quieto un rato despues de la escena, ya recibe daño

    public event Action OnShoutStarted;
    public event Action OnPhaseChanged;

    public void StartChange()
    {
        isChanging = true;
        isShouting = true;
        OnShoutStarted?.Invoke();
    }

    // Termina el grito y aparece la espada en llamas
    public void EnterPhaseTwo()
    {
        phase = 2;
        isShouting = false;
        OnPhaseChanged?.Invoke();
    }

    public void EndChange()
    {
        isChanging = false;
        isRecovering = true;
    }

    public void EndRecovery()
    {
        isRecovering = false;
    }

    // Vuelve a la primera fase cuando el jugador reaparece
    public void ResetPhase()
    {
        phase = 1;
        isChanging = false;
        isShouting = false;
        isRecovering = false;
        OnPhaseChanged?.Invoke();
    }

    // Cuanto queda de la barra de esta fase, de 0 a 1
    public float GetFraction(int current)
    {
        int phaseTwoHealth = bossData.GetPhaseTwoHealth();
        if (phase > 1) return phaseTwoHealth > 0 ? Mathf.Clamp01((float)current / phaseTwoHealth) : 0f;

        int phaseOneHealth = bossData.GetMaxHealth() - phaseTwoHealth;
        return phaseOneHealth > 0 ? Mathf.Clamp01((float)(current - phaseTwoHealth) / phaseOneHealth) : 0f;
    }

    public string GetStateName(string baseName) => phase > 1 ? baseName + FlamingSuffix : baseName;
    public float GetSpeedMultiplier() => phase > 1 ? bossData.GetPhaseTwoSpeedMultiplier() : 1f;
    public float GetPatternCooldown() => phase > 1 ? bossData.GetPhaseTwoPatternCooldown() : bossData.GetAttackCooldown();
    public float GetStunDuration() => phase > 1 ? bossData.GetPhaseTwoStunDuration() : bossData.GetStunDuration();
    public int GetPhaseTwoHealth() => bossData.GetPhaseTwoHealth();
    public bool GetIsPhaseTwo() => phase > 1;
    public bool GetIsChanging() => isChanging;
    public bool GetIsHoldingStill() => isChanging || isRecovering;
    public bool GetIsShouting() => isShouting;
}
