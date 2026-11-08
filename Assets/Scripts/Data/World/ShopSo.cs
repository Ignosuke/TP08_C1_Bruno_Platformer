using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// La tienda del mercader. Las compras duran solo la partida
[CreateAssetMenu(fileName = "ShopSo", menuName = "Data/Shop")]
public class ShopSo : ScriptableObject
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Items")]
    [Tooltip("Sharpen purchase N adds damage to combo step N")]
    [SerializeField] private List<ShopItem> items = new List<ShopItem>
    {
        new ShopItem("Max Health", ShopItemType.MaxHealth, 2, 15, 22),
        new ShopItem("Sharpen", ShopItemType.Sharpen, 1, 10, 15, 20),
        new ShopItem("Heal Charge", ShopItemType.HealCharge, 1, 8, 12),
        new ShopItem("Shurikens +3", ShopItemType.Shuriken, 3, 5, 6, 7),
    };

    [Header("Sounds")]
    [SerializeField] private AudioClip buyClip;
    [SerializeField] private AudioClip denyClip;                // Sin monedas o ya comprado al maximo

    public InputActionReference GetInteractAction() => interactAction;
    public InputActionReference GetNavigateAction() => navigateAction;
    public InputActionReference GetSubmitAction() => submitAction;
    public InputActionReference GetCancelAction() => cancelAction;

    public int GetItemCount() => items.Count;
    public ShopItem GetItem(int index) => items[index];

    public AudioClip GetBuyClip() => buyClip;
    public AudioClip GetDenyClip() => denyClip;
}
