using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Base de todos los paneles, se abre y se cierra prendiendo el objeto. Al abrir selecciona su primer boton para el teclado
public class UiPanel : MonoBehaviour
{
    [Tooltip("Selected on open so keyboard and gamepad can navigate, can be empty")]
    [SerializeField] private Selectable firstSelected;

    public bool GetIsOpen() => gameObject.activeSelf;

    public void Open()
    {
        gameObject.SetActive(true);
        if (firstSelected != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
