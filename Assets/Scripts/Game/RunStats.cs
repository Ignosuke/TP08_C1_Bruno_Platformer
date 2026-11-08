using System;
using UnityEngine;

// Junta los datos de la partida para el puntaje. El tiempo corre desde que arranca la escena y no suma en pausa,
// al vencer al jefe calcula el total, guarda el record si lo supera y avisa una sola vez
public class RunStats : MonoBehaviour
{
    private const string BestScoreKey = "BestScore";

    [SerializeField] private ScoreSo scoreData;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private PlayerWallet wallet;
    [SerializeField] private BossBrain boss;

    private float seconds;
    private int parries;
    private int deaths;
    private bool isOver;

    public event Action<ScoreResult> OnRunScored;

    private void OnEnable()
    {
        player.OnParried += CountParry;
        player.OnDied += CountDeath;
        boss.OnDefeated += Score;
    }

    private void OnDisable()
    {
        player.OnParried -= CountParry;
        player.OnDied -= CountDeath;
        boss.OnDefeated -= Score;
    }

    // Con la pausa el timeScale queda en 0 y no suma
    private void Update()
    {
        if (!isOver) seconds += Time.deltaTime;
    }

    private void CountParry(DamageInfo info)
    {
        parries++;
    }

    private void CountDeath()
    {
        deaths++;
    }

    private void Score()
    {
        if (isOver) return;
        isOver = true;

        int coins = wallet.GetCollected();
        int timePoints = scoreData.GetTimePoints(seconds);
        int coinPoints = scoreData.GetCoinPoints(coins);
        int parryPoints = scoreData.GetParryPoints(parries);
        float deathMultiplier = scoreData.GetDeathMultiplier(deaths);
        int total = scoreData.GetTotal(timePoints, coinPoints, parryPoints, deathMultiplier);

        int best = GetBestScore();
        bool isNewBest = total > best;
        if (isNewBest)
        {
            best = total;
            PlayerPrefs.SetInt(BestScoreKey, total);
            PlayerPrefs.Save();
        }

        OnRunScored?.Invoke(new ScoreResult(seconds, timePoints, coins, coinPoints, parries, parryPoints,
            deaths, deathMultiplier, total, best, isNewBest));
    }

    // Para que el menu muestre el record, 0 si nunca se termino una partida
    public static int GetBestScore() => PlayerPrefs.GetInt(BestScoreKey, 0);
}
