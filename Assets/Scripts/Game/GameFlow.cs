using System;
using UnityEngine;
using UnityEngine.InputSystem;

// El flujo de la partida con la FSM compartida. Jugando queda abajo y la pausa se apila encima, al sacarla vuelve a jugar.
// Avisa con eventos, la UI escucha y abre o cierra sus paneles
public class GameFlow : MonoBehaviour
{
    private const string PlayerMapName = "Player";

    [SerializeField] private InputActionAsset inputActions;
    [Tooltip("Disabled while dead, in the shop or in the boss cutscene, then pause is not allowed")]
    [SerializeField] private PlayerMover player;
    [Tooltip("Optional, after its defeat pause is not allowed")]
    [SerializeField] private BossBrain boss;

    private StateMachine stateMachine;
    private PlayingState playing;
    private PausedState paused;
    private bool isOver;

    public event Action OnPaused;
    public event Action OnResumed;

    private void Awake()
    {
        stateMachine = new StateMachine();
        playing = new PlayingState();
        paused = new PausedState(inputActions.FindActionMap(PlayerMapName, true));
        stateMachine.ChangeState(playing);
    }

    private void OnEnable()
    {
        if (boss != null) boss.OnDefeated += End;
    }

    // Si la escena se descarga en pausa igual devuelve el tiempo y los controles
    private void OnDisable()
    {
        if (boss != null) boss.OnDefeated -= End;
        if (GetIsPaused()) stateMachine.PopState();
    }

    private void End()
    {
        isOver = true;
    }

    // Los controles apagados ya dicen que el jugador esta muerto, comprando o en la escena del jefe
    public void Pause()
    {
        if (GetIsPaused() || isOver || !player.enabled) return;

        stateMachine.PushState(paused);
        OnPaused?.Invoke();
    }

    public void Resume()
    {
        if (!GetIsPaused()) return;

        stateMachine.PopState();
        OnResumed?.Invoke();
    }

    public bool GetIsPaused() => stateMachine.GetCurrent() == paused;
}
