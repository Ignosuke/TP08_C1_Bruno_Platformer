using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// El puntaje al ganar. Las filas aparecen de a una, despues el total y el record, y al final los botones.
// Lo abre VictoryBanner con el resultado de RunStats, todo en tiempo real
[RequireComponent(typeof(UiPanel))]
public class UiVictory : MonoBehaviour
{
    // Suggestion: los nombres de escena estan repetidos como string en UiVictory, UiMainMenuCanvas, UiGameCanvas y UiSplash.
    // Si se renombra una escena hay que tocar 4 componentes en 3 escenas. Centralizalos en una clase de constantes o en un SO.
    [Tooltip("Must match scenes in the build settings")]
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private float rowDelay = 0.35f;        // Segundos reales entre fila y fila
    [SerializeField] private float totalDelay = 0.6f;       // Una espera mas larga antes del total

    [Header("Sound")]
    [SerializeField] private UiClickPlayer clickPlayer;
    [SerializeField] private float rowPitch = 1.3f;
    [SerializeField] private float totalPitch = 1.5f;
    [SerializeField] private float newBestPitch = 1.7f;

    [Header("Rows")]
    [SerializeField] private UiScoreRow timeRow;
    [SerializeField] private UiScoreRow coinsRow;
    [SerializeField] private UiScoreRow parriesRow;
    [SerializeField] private UiScoreRow deathsRow;
    [SerializeField] private UiScoreRow totalRow;
    [SerializeField] private UiPopIn newBest;

    [Header("Buttons")]
    [SerializeField] private UiPopIn buttons;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    private ScoreResult result;

    public void SetResult(ScoreResult score)
    {
        result = score;
    }

    private void OnEnable()
    {
        replayButton.onClick.AddListener(Replay);
        menuButton.onClick.AddListener(GoToMenu);
        StartCoroutine(Reveal());
    }

    private void OnDisable()
    {
        replayButton.onClick.RemoveListener(Replay);
        menuButton.onClick.RemoveListener(GoToMenu);
    }

    private IEnumerator Reveal()
    {
        timeRow.Hide();
        coinsRow.Hide();
        parriesRow.Hide();
        deathsRow.Hide();
        totalRow.Hide();
        newBest.Hide();
        buttons.Hide();

        yield return new WaitForSecondsRealtime(rowDelay);
        timeRow.Show(FormatTime(result.Seconds), result.TimePoints.ToString());
        PlayClick(rowPitch);
        yield return new WaitForSecondsRealtime(rowDelay);
        coinsRow.Show(result.Coins.ToString(), result.CoinPoints.ToString());
        PlayClick(rowPitch);
        yield return new WaitForSecondsRealtime(rowDelay);
        parriesRow.Show(result.Parries.ToString(), result.ParryPoints.ToString());
        PlayClick(rowPitch);
        yield return new WaitForSecondsRealtime(rowDelay);
        // Invariante para que salga x1.2 y no x1,2 con la PC en español
        deathsRow.Show(result.Deaths.ToString(), "x" + result.DeathMultiplier.ToString("0.0#", CultureInfo.InvariantCulture));
        PlayClick(rowPitch);
        yield return new WaitForSecondsRealtime(totalDelay);
        totalRow.Show("", result.Total.ToString());
        PlayClick(totalPitch);

        if (result.IsNewBest)
        {
            yield return new WaitForSecondsRealtime(rowDelay);
            newBest.Play();
            PlayClick(newBestPitch);
        }

        yield return new WaitForSecondsRealtime(rowDelay);
        buttons.Play();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(replayButton.gameObject);
    }

    private void PlayClick(float pitch)
    {
        if (clickPlayer != null) clickPlayer.Play(pitch);
    }

    // Minutos y segundos, siempre con dos cifras
    private string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }

    private void Replay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
