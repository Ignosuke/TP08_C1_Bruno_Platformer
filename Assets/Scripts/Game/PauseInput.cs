using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Escape, P o Start del joystick avisan que se pidio la pausa, K o el boton de la derecha que se pidio volver.
// Las acciones se arman aca porque el asset de input no las tiene. Volver es aparte porque jugando esas teclas son de otra accion
public class PauseInput : MonoBehaviour
{
    private InputAction pauseAction;
    private InputAction backAction;

    public event Action OnPressed;
    public event Action OnBackPressed;

    private void Awake()
    {
        pauseAction = new InputAction("Pause", InputActionType.Button);
        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Keyboard>/p");
        pauseAction.AddBinding("<Gamepad>/start");

        backAction = new InputAction("Back", InputActionType.Button);
        backAction.AddBinding("<Keyboard>/k");
        backAction.AddBinding("<Gamepad>/buttonEast");
    }

    private void OnEnable()
    {
        pauseAction.performed += OnPausePerformed;
        pauseAction.Enable();
        backAction.performed += OnBackPerformed;
        backAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
        backAction.performed -= OnBackPerformed;
        backAction.Disable();
    }

    private void OnDestroy()
    {
        pauseAction.Dispose();
        backAction.Dispose();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        OnPressed?.Invoke();
    }

    private void OnBackPerformed(InputAction.CallbackContext context)
    {
        OnBackPressed?.Invoke();
    }
}
