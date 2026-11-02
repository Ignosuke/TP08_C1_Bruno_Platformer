using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Botones del menu principal, Play carga la escena de juego y Quit cierra la aplicacion
public class MainMenu : MonoBehaviour
{
    [Tooltip("Must match a scene in the build settings")]
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // En la web Application.Quit no hace nada, asi que el boton no se muestra
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            quitButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(Play);
        quitButton.onClick.RemoveListener(Quit);
    }

    private void Play()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
