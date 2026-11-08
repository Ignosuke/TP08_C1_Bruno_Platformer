// El puntaje de una partida terminada, lo manda RunStats al vencer al jefe
public readonly struct ScoreResult
{
    public readonly float Seconds;
    public readonly int TimePoints;
    public readonly int Coins;
    public readonly int CoinPoints;
    public readonly int Parries;
    public readonly int ParryPoints;
    public readonly int Deaths;
    public readonly float DeathMultiplier;
    public readonly int Total;
    public readonly int Best;           // Ya incluye esta partida si fue record
    public readonly bool IsNewBest;

    public ScoreResult(float seconds, int timePoints, int coins, int coinPoints, int parries, int parryPoints,
        int deaths, float deathMultiplier, int total, int best, bool isNewBest)
    {
        Seconds = seconds;
        TimePoints = timePoints;
        Coins = coins;
        CoinPoints = coinPoints;
        Parries = parries;
        ParryPoints = parryPoints;
        Deaths = deaths;
        DeathMultiplier = deathMultiplier;
        Total = total;
        Best = best;
        IsNewBest = isNewBest;
    }
}
