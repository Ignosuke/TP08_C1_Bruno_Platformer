using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// El canvas de la partida, tiene todo el HUD y los paneles de pausa. Escucha a GameFlow y a los botones,
// y muestra un panel por vez solo con Open y Close. No pausa por su cuenta, se lo pide a GameFlow
public class UiGameCanvas : MonoBehaviour
{
    [SerializeField] private GameFlow gameFlow;
    [SerializeField] private PauseInput pauseInput;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Panels")]
    [SerializeField] private UiPanel pausePanel;
    [SerializeField] private UiPanel settingsPanel;
    [SerializeField] private UiPanel creditsPanel;

    [Header("Pause Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button menuButton;

    [Header("Back Buttons")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button creditsBackButton;

    private void Awake()
    {
        CloseAll();
    }

    private void OnEnable()
    {
        gameFlow.OnPaused += ShowPause;
        gameFlow.OnResumed += CloseAll;
        pauseInput.OnPressed += OnPausePressed;
        pauseInput.OnBackPressed += OnBackPressed;

        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(ShowSettings);
        creditsButton.onClick.AddListener(ShowCredits);
        menuButton.onClick.AddListener(GoToMenu);
        settingsBackButton.onClick.AddListener(ShowPause);
        creditsBackButton.onClick.AddListener(ShowPause);
    }

    private void OnDisable()
    {
        gameFlow.OnPaused -= ShowPause;
        gameFlow.OnResumed -= CloseAll;
        pauseInput.OnPressed -= OnPausePressed;
        pauseInput.OnBackPressed -= OnBackPressed;

        resumeButton.onClick.RemoveListener(Resume);
        settingsButton.onClick.RemoveListener(ShowSettings);
        creditsButton.onClick.RemoveListener(ShowCredits);
        menuButton.onClick.RemoveListener(GoToMenu);
        settingsBackButton.onClick.RemoveListener(ShowPause);
        creditsBackButton.onClick.RemoveListener(ShowPause);
    }

    // Jugando pide la pausa, en la pausa la saca, y en Settings o Credits vuelve a la pausa
    private void OnPausePressed()
    {
        if (!gameFlow.GetIsPaused()) gameFlow.Pause();
        else if (pausePanel.GetIsOpen()) gameFlow.Resume();
        else ShowPause();
    }

    // K vuelve igual que Escape, pero solo en la pausa, jugando es el dash
    private void OnBackPressed()
    {
        if (gameFlow.GetIsPaused()) OnPausePressed();
    }

    private void ShowPause() => ShowOnly(pausePanel);
    private void ShowSettings() => ShowOnly(settingsPanel);
    private void ShowCredits() => ShowOnly(creditsPanel);

    private void ShowOnly(UiPanel panel)
    {
        CloseAll();
        panel.Open();
    }

    private void CloseAll()
    {
        pausePanel.Close();
        settingsPanel.Close();
        creditsPanel.Close();
    }

    private void Resume()
    {
        gameFlow.Resume();
    }

    // Primero sale de la pausa, asi el menu arranca con el tiempo y los controles normales
    private void GoToMenu()
    {
        gameFlow.Resume();
        SceneManager.LoadScene(menuSceneName);
    }
}
