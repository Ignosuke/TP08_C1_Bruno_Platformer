using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

// Confirmar en los menus con J, igual que comprar en la tienda. El asset de input no la tiene en Submit,
// asi que la accion se arma aca. No se usa Player/Attack porque tiene el click del mouse y se apaga en la pausa
[RequireComponent(typeof(InputSystemUIInputModule))]
public class UiSubmitInput : MonoBehaviour
{
    private void Awake()
    {
        // La referencia de una accion necesita un asset, se crea uno chico solo para esta
        InputActionAsset actions = ScriptableObject.CreateInstance<InputActionAsset>();
        InputAction submit = actions.AddActionMap("Ui").AddAction("Submit", InputActionType.Button);
        submit.AddBinding("<Keyboard>/j");
        submit.AddBinding("*/{Submit}");        // Enter y el boton de abajo del joystick siguen andando

        GetComponent<InputSystemUIInputModule>().submit = InputActionReference.Create(submit);
    }
}
