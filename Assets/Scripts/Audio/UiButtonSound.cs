using UnityEngine;
using UnityEngine.UI;

// Va en el prefab UiButton, cada click pide el sonido al UiClickPlayer de la escena
[RequireComponent(typeof(Button))]
public class UiButtonSound : MonoBehaviour
{
    [SerializeField] private UiClickPlayer clickPlayer;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(PlayClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(PlayClick);
    }

    private void PlayClick()
    {
        if (clickPlayer != null) clickPlayer.Play();
    }
}
