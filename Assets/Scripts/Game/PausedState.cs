using UnityEngine;
using UnityEngine.InputSystem;

// Congela el tiempo y apaga las acciones del jugador, al salir las devuelve.
// HitStop no toca un timeScale 0, asi que un golpe que termina no rompe la pausa
public class PausedState : State
{
    private readonly InputActionMap playerMap;

    public PausedState(InputActionMap playerMap)
    {
        this.playerMap = playerMap;
    }

    public override void Enter()
    {
        Time.timeScale = 0f;
        playerMap.Disable();
    }

    public override void Exit()
    {
        Time.timeScale = 1f;
        playerMap.Enable();
    }
}
