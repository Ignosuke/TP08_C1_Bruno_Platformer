using TMPro;
using UnityEngine;

// Las monedas del jugador en el HUD, como un numero
public class CoinCounter : MonoBehaviour
{
    [SerializeField] private PlayerWallet wallet;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        wallet.OnCoinsChanged += Refresh;
    }

    private void OnDisable()
    {
        wallet.OnCoinsChanged -= Refresh;
    }

    private void Start()
    {
        Refresh(wallet.GetCoins());
    }

    private void Refresh(int coins)
    {
        text.text = coins.ToString();
    }
}
