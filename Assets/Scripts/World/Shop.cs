using System;
using UnityEngine;

// Las compras de la tienda. Cobra, aplica la mejora al jugador y cuenta cuantas veces se compro cada articulo.
// Las compras duran solo la partida
public class Shop : MonoBehaviour
{
    [SerializeField] private ShopSo shopData;
    [SerializeField] private PlayerHealth player;

    private PlayerWallet wallet;
    private PlayerHealer healer;
    private PlayerAttacker attacker;
    private PlayerThrower thrower;
    private PlayerAudio playerAudio;
    private int[] levels;               // Compras hechas de cada articulo

    public event Action OnPurchased;

    private void Awake()
    {
        wallet = player.GetComponent<PlayerWallet>();
        healer = player.GetComponent<PlayerHealer>();
        attacker = player.GetComponent<PlayerAttacker>();
        thrower = player.GetComponent<PlayerThrower>();
        playerAudio = player.GetComponent<PlayerAudio>();
        levels = new int[shopData.GetItemCount()];
    }

    public bool TryBuy(int index)
    {
        if (!GetCanBuy(index))
        {
            playerAudio.Play(shopData.GetDenyClip());
            return false;
        }

        ShopItem item = shopData.GetItem(index);
        wallet.Spend(item.GetPrice(levels[index]));
        Apply(item, levels[index]);
        levels[index]++;

        playerAudio.Play(shopData.GetBuyClip());
        OnPurchased?.Invoke();
        return true;
    }

    // level es la compra que se esta haciendo, empieza en 0. El afilado N mejora el golpe N del combo
    private void Apply(ShopItem item, int level)
    {
        switch (item.GetItemType())
        {
            case ShopItemType.MaxHealth:
                player.IncreaseMax(item.GetAmount());
                break;
            case ShopItemType.Sharpen:
                attacker.AddComboDamage(level, item.GetAmount());
                break;
            case ShopItemType.HealCharge:
                healer.AddMaxCharges(item.GetAmount());
                break;
            case ShopItemType.Shuriken:
                thrower.AddAmmo(item.GetAmount());
                break;
        }
    }

    public int GetItemCount() => levels.Length;
    public ShopItem GetItem(int index) => shopData.GetItem(index);
    public int GetLevel(int index) => levels[index];
    public bool GetIsMaxed(int index) => levels[index] >= shopData.GetItem(index).GetMaxLevel();
    public bool GetCanBuy(int index) => !GetIsMaxed(index) && wallet.GetCoins() >= shopData.GetItem(index).GetPrice(levels[index]);
}
