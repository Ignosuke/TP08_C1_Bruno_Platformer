using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// El canvas del menu principal. Escucha los botones y muestra un panel por vez, solo con Open y Close
public class UiMainMenuCanvas : MonoBehaviour
{
    [Tooltip("Must match a scene in the build settings")]
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [Tooltip("Escape or K go back from Settings and Credits")]
    [SerializeField] private PauseInput backInput;

    [Header("Panels")]
    [SerializeField] private UiPanel mainMenuPanel;
    [SerializeField] private UiPanel settingsPanel;
    [SerializeField] private UiPanel creditsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [Header("Back Buttons")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button creditsBackButton;

    private void Awake()
    {
        // En la web Application.Quit no hace nada, asi que el boton no se muestra
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            exitButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(Play);
        settingsButton.onClick.AddListener(ShowSettings);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(Exit);
        settingsBackButton.onClick.AddListener(ShowMainMenu);
        creditsBackButton.onClick.AddListener(ShowMainMenu);
        backInput.OnPressed += Back;
        backInput.OnBackPressed += Back;
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(Play);
        settingsButton.onClick.RemoveListener(ShowSettings);
        creditsButton.onClick.RemoveListener(ShowCredits);
        exitButton.onClick.RemoveListener(Exit);
        settingsBackButton.onClick.RemoveListener(ShowMainMenu);
        creditsBackButton.onClick.RemoveListener(ShowMainMenu);
        backInput.OnPressed -= Back;
        backInput.OnBackPressed -= Back;
    }

    // En Start el EventSystem ya esta prendido y el panel puede seleccionar su primer boton
    private void Start()
    {
        ShowMainMenu();
    }

    // En el panel principal no hay a donde volver
    private void Back()
    {
        if (!mainMenuPanel.GetIsOpen()) ShowMainMenu();
    }

    private void ShowMainMenu() => ShowOnly(mainMenuPanel);
    private void ShowSettings() => ShowOnly(settingsPanel);
    private void ShowCredits() => ShowOnly(creditsPanel);

    private void ShowOnly(UiPanel panel)
    {
        mainMenuPanel.Close();
        settingsPanel.Close();
        creditsPanel.Close();
        panel.Open();
    }

    private void Play()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
