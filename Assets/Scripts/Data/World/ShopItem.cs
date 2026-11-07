using System;
using UnityEngine;

public enum ShopItemType { MaxHealth, Sharpen, HealCharge }

// Un articulo de la tienda. Se puede comprar tantas veces como precios tenga, cada compra cuesta el precio siguiente
[Serializable]
public class ShopItem
{
    [SerializeField] private string displayName;
    [SerializeField] private ShopItemType type;
    [Tooltip("Max health in half hearts, damage per sharpen or heal charges")]
    [SerializeField] private int amount;
    [Tooltip("One price per purchase, the length is the purchase cap")]
    [SerializeField] private int[] prices = new int[0];

    // Para que Unity pueda crear uno vacio desde el Inspector
    public ShopItem() { }

    public ShopItem(string displayName, ShopItemType type, int amount, params int[] prices)
    {
        this.displayName = displayName;
        this.type = type;
        this.amount = amount;
        this.prices = prices;
    }

    public string GetDisplayName() => displayName;
    public ShopItemType GetItemType() => type;
    public int GetAmount() => amount;
    public int GetMaxLevel() => prices.Length;
    public int GetPrice(int level) => prices[level];
}
