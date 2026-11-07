using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cuando cae el jefe muestra la victoria unos segundos y vuelve al menu
public class VictoryBanner : MonoBehaviour
{
    [SerializeField] private BossBrain brain;
    [SerializeField] private GameObject banner;
    [Tooltip("Real seconds the banner stays before loading the menu")]
    [SerializeField] private float showTime = 4f;
    [SerializeField] private string menuSceneName = "MainMenu";

    private void OnEnable()
    {
        banner.SetActive(false);
        brain.OnDefeated += Show;
    }

    private void OnDisable()
    {
        brain.OnDefeated -= Show;
    }

    private void Show()
    {
        StartCoroutine(ShowThenLeave());
    }

    private IEnumerator ShowThenLeave()
    {
        banner.SetActive(true);
        yield return new WaitForSecondsRealtime(showTime);
        SceneManager.LoadScene(menuSceneName);
    }
}
