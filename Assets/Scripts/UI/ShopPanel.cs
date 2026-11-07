using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// La ventana de la tienda en el HUD. Arriba y abajo eligen, confirmar compra, cancelar o interactuar cierra.
// Esta apagada hasta que el mercader la abre, al prenderse escucha las teclas y al apagarse las suelta
public class ShopPanel : MonoBehaviour
{
    private const float NavigateThreshold = 0.5f;   // Cuanto hay que inclinar el stick para moverse

    [SerializeField] private ShopSo shopData;
    [SerializeField] private Shop shop;
    [Tooltip("One text per shop item, in the same order")]
    [SerializeField] private TMP_Text[] rows;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.45f);
    [SerializeField] private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f);

    private InputActionReference navigateAction;
    private InputActionReference submitAction;
    private InputActionReference cancelAction;
    private InputActionReference interactAction;

    private int selected;
    private int lastDirection;          // Para moverse una vez por toque y no cada frame
    private int openedFrame;            // La tecla que abrio la tienda no la cierra en el mismo frame
    private bool submitRequested;
    private bool closeRequested;

    public event Action OnClosed;

    private void Awake()
    {
        navigateAction = shopData.GetNavigateAction();
        submitAction = shopData.GetSubmitAction();
        cancelAction = shopData.GetCancelAction();
        interactAction = shopData.GetInteractAction();
    }

    private void OnEnable()
    {
        submitAction.action.performed += OnSubmitPerformed;
        cancelAction.action.performed += OnClosePressed;
        interactAction.action.started += OnClosePressed;
        navigateAction.action.Enable();
        submitAction.action.Enable();
        cancelAction.action.Enable();

        selected = 0;
        lastDirection = 0;
        openedFrame = Time.frameCount;
        submitRequested = false;
        closeRequested = false;
        Refresh();
    }

    // Comprar y salir son acciones del jugador (atacar y dash), igual que Interact no se apagan
    private void OnDisable()
    {
        submitAction.action.performed -= OnSubmitPerformed;
        cancelAction.action.performed -= OnClosePressed;
        interactAction.action.started -= OnClosePressed;
        navigateAction.action.Disable();
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        submitRequested = true;
    }

    private void OnClosePressed(InputAction.CallbackContext context)
    {
        closeRequested = true;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClosed?.Invoke();
    }

    private void Update()
    {
        if (Time.frameCount == openedFrame)
        {
            submitRequested = false;
            closeRequested = false;
            return;
        }

        if (closeRequested)
        {
            Close();
            return;
        }

        // Arriba en el stick es positivo y la lista va de arriba hacia abajo
        float vertical = navigateAction.action.ReadValue<Vector2>().y;
        int direction = vertical > NavigateThreshold ? -1 : vertical < -NavigateThreshold ? 1 : 0;
        if (direction != 0 && direction != lastDirection)
        {
            selected = (selected + direction + rows.Length) % rows.Length;
            Refresh();
        }
        lastDirection = direction;

        if (submitRequested)
        {
            submitRequested = false;
            shop.TryBuy(selected);
            Refresh();
        }
    }

    // Nombre, compras hechas sobre el maximo y precio, o MAX si ya no se puede comprar mas
    private void Refresh()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            ShopItem item = shop.GetItem(i);
            int level = shop.GetLevel(i);
            string price = shop.GetIsMaxed(i) ? "MAX" : item.GetPrice(level).ToString();
            string cursor = i == selected ? "> " : "  ";

            rows[i].text = cursor + item.GetDisplayName() + "<pos=60%>" + level + "/" + item.GetMaxLevel() + "<pos=80%>" + price;
            rows[i].color = i == selected ? selectedColor : shop.GetCanBuy(i) ? normalColor : unavailableColor;
        }
    }

    public bool GetIsOpen() => gameObject.activeSelf;
}
