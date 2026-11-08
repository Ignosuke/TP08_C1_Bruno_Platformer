using UnityEngine;

// Los valores del puntaje de la partida y las cuentas que los usan.
// Pensado para una partida de 3 a 6 minutos, 3 minutos dan 6400 de tiempo y 6 minutos 2800
[CreateAssetMenu(fileName = "ScoreSo", menuName = "Data/Score")]
public class ScoreSo : ScriptableObject
{
    [Header("Time")]
    [SerializeField] private int timeBasePoints = 10000;        // Lo que daria terminar en 0 segundos
    [SerializeField] private float timePointsPerSecond = 20f;
    [Tooltip("Time points never go below this, reached at 7.5 minutes with the default values")]
    [SerializeField] private int timeMinPoints = 1000;

    [Header("Coins and Parries")]
    [Tooltip("Points per coin collected in the run, spending does not take them away")]
    [SerializeField] private int pointsPerCoin = 25;
    [SerializeField] private int pointsPerParry = 100;

    [Header("Deaths")]
    [SerializeField] private float noDeathMultiplier = 1.2f;
    [SerializeField] private float oneDeathMultiplier = 1f;
    [Tooltip("Taken off the multiplier for each death after the first")]
    [SerializeField] private float multiplierPerExtraDeath = 0.1f;
    [Tooltip("The multiplier never goes below this")]
    [SerializeField] private float minDeathMultiplier = 0.5f;

    public int GetTimePoints(float seconds) => Mathf.Max(timeBasePoints - Mathf.FloorToInt(seconds * timePointsPerSecond), timeMinPoints);
    public int GetCoinPoints(int coins) => coins * pointsPerCoin;
    public int GetParryPoints(int parries) => parries * pointsPerParry;

    // Sin muertes premia, con una queda igual y cada muerte de mas resta, con un piso
    public float GetDeathMultiplier(int deaths)
    {
        if (deaths <= 0) return noDeathMultiplier;
        return Mathf.Max(oneDeathMultiplier - (deaths - 1) * multiplierPerExtraDeath, minDeathMultiplier);
    }

    public int GetTotal(int timePoints, int coinPoints, int parryPoints, float deathMultiplier) =>
        Mathf.RoundToInt((timePoints + coinPoints + parryPoints) * deathMultiplier);
}
