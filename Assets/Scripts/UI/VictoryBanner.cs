using System.Collections;
using UnityEngine;

// Cuando RunStats termina el puntaje muestra la victoria unos segundos y despues abre el panel con el puntaje
public class VictoryBanner : MonoBehaviour
{
    [SerializeField] private RunStats runStats;
    [SerializeField] private GameObject banner;
    [SerializeField] private UiPanel victoryPanel;
    [SerializeField] private UiVictory victory;
    [Tooltip("Real seconds the banner stays before the score")]
    [SerializeField] private float showTime = 4f;

    private void OnEnable()
    {
        banner.SetActive(false);
        victoryPanel.Close();
        runStats.OnRunScored += Show;
    }

    private void OnDisable()
    {
        runStats.OnRunScored -= Show;
    }

    private void Show(ScoreResult result)
    {
        StartCoroutine(ShowThenScore(result));
    }

    // Tiempo real por si el ultimo golpe dejo el hitstop
    private IEnumerator ShowThenScore(ScoreResult result)
    {
        banner.SetActive(true);
        yield return new WaitForSecondsRealtime(showTime);
        banner.SetActive(false);
        victory.SetResult(result);
        victoryPanel.Open();
    }
}
