using System;
using UnityEngine;

// Las monedas de la partida. Se quedan al morir y se pierden cuando termina la partida (recarga de escena o menu).
// El record es lo mas que se junto en una partida, no se gasta
public class PlayerWallet : MonoBehaviour
{
    private const string BestCoinsKey = "BestRunCoins";

    private int coins;
    private int collected;              // Todo lo juntado en la partida, aunque se haya gastado

    public event Action<int> OnCoinsChanged;   // Manda las monedas actuales

    public void Add(int amount)
    {
        coins += amount;
        collected += amount;
        OnCoinsChanged?.Invoke(coins);
    }

    // Devuelve false si no alcanza, asi no se gasta nada
    public bool Spend(int amount)
    {
        if (amount > coins) return false;

        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    // Se destruye al cambiar de escena, al ganar y al volver al menu la partida termino
    private void OnDestroy()
    {
        if (collected <= GetBestCoins()) return;

        PlayerPrefs.SetInt(BestCoinsKey, collected);
        PlayerPrefs.Save();
    }

    public int GetCoins() => coins;
    public int GetCollected() => collected;   // Para el puntaje, lo gastado tambien cuenta

    // Para que el menu muestre el record
    // Suggestion: el menu no lo muestra (UiBestScore solo lee RunStats.GetBestScore), asi que BestRunCoins se guarda y nunca se ve.
    // Mostralo en el menu o saca el comentario.
    public static int GetBestCoins() => PlayerPrefs.GetInt(BestCoinsKey, 0);
}
