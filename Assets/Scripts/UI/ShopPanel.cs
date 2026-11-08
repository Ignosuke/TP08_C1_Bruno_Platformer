using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// La ventana de la tienda en el HUD. Arriba y abajo eligen, confirmar compra, cancelar o interactuar cierra.
// Esta apagada hasta que el mercader la abre, al prenderse escucha las teclas y al apagarse las suelta
public class ShopPanel : MonoBehaviour
{
    private const float NavigateThreshold = 0.5f;   // Cuanto hay que inclinar el stick para moverse
    private const string NameStart = "<pos=30>";   // El nombre arranca siempre en el mismo lugar, con o sin icono

    [SerializeField] private ShopSo shopData;
    [SerializeField] private Shop shop;
    [Tooltip("One text per shop item, in the same order")]
    [SerializeField] private TMP_Text[] rows;
    [Tooltip("Key hint panel at the bottom right, shown only while the shop is open")]
    [SerializeField] private GameObject hint;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.45f);
    [SerializeField] private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f);
    [Tooltip("Item icon between the cursor and the name, in HUD pixels")]
    [SerializeField] private Vector2 iconSize = new Vector2(16f, 16f);
    [SerializeField] private float iconX = 12f;     // Justo despues del cursor

    private InputActionReference navigateAction;
    private InputActionReference submitAction;
    private InputActionReference cancelAction;
    private InputActionReference interactAction;
    private Image[] icons;

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

        // Los iconos se crean aca, asi las filas de la escena no cambian
        icons = new Image[rows.Length];
        for (int i = 0; i < rows.Length; i++) icons[i] = CreateIcon(rows[i].rectTransform);
    }

    private Image CreateIcon(RectTransform row)
    {
        GameObject iconObject = new GameObject("Icon", typeof(RectTransform));
        iconObject.transform.SetParent(row, false);
        Image icon = iconObject.AddComponent<Image>();
        icon.raycastTarget = false;
        icon.preserveAspect = true;

        RectTransform rect = icon.rectTransform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(iconX, 0f);
        rect.sizeDelta = iconSize;
        return icon;
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
        if (hint != null) hint.SetActive(true);
        Refresh();
    }

    // Comprar y salir son acciones del jugador (atacar y dash), igual que Interact no se apagan
    private void OnDisable()
    {
        submitAction.action.performed -= OnSubmitPerformed;
        cancelAction.action.performed -= OnClosePressed;
        interactAction.action.started -= OnClosePressed;
        navigateAction.action.Disable();
        if (hint != null) hint.SetActive(false);
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

    // Icono, nombre, compras hechas sobre el maximo y precio, o MAX si ya no se puede comprar mas
    private void Refresh()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            ShopItem item = shop.GetItem(i);
            int level = shop.GetLevel(i);
            string price = shop.GetIsMaxed(i) ? "MAX" : item.GetPrice(level).ToString();
            string cursor = i == selected ? "> " : "  ";

            rows[i].text = cursor + NameStart + item.GetDisplayName() + "<pos=60%>" + level + "/" + item.GetMaxLevel() + "<pos=80%>" + price;
            rows[i].color = i == selected ? selectedColor : shop.GetCanBuy(i) ? normalColor : unavailableColor;

            // El icono muestra la proxima compra, si ya esta al maximo o no tiene icono se oculta
            Sprite icon = item.GetIcon(level);
            icons[i].sprite = icon;
            icons[i].enabled = icon != null && !shop.GetIsMaxed(i);
        }
    }

    public bool GetIsOpen() => gameObject.activeSelf;
}
