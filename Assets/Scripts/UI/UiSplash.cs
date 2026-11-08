using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

// La pantalla de entrada. El titulo aparece, se queda y se va, despues carga el menu. Cualquier tecla o boton la saltea
public class UiSplash : MonoBehaviour
{
    [Tooltip("Must match a scene in the build settings")]
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private CanvasGroup title;
    [SerializeField] private float fadeInTime = 1f;         // Segundos reales
    [SerializeField] private float holdTime = 1.5f;
    [SerializeField] private float fadeOutTime = 1f;

    private IDisposable anyButtonListener;
    private bool isLeaving;

    private void OnEnable()
    {
        anyButtonListener = InputSystem.onAnyButtonPress.CallOnce(OnAnyButton);
    }

    private void OnDisable()
    {
        anyButtonListener?.Dispose();
    }

    private void Start()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSecondsRealtime(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);
        Leave();
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
        {
            title.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }
        title.alpha = to;
    }

    private void OnAnyButton(InputControl control)
    {
        Leave();
    }

    // La tecla y el final del fundido pueden llegar juntos, se carga una sola vez
    private void Leave()
    {
        if (isLeaving) return;
        isLeaving = true;
        SceneManager.LoadScene(menuSceneName);
    }
}
