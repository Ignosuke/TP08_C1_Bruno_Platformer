using TMPro;
using UnityEngine;

// El cartel del HUD que usan las zonas de tutorial. Esta apagado hasta que una zona lo muestra
public class PromptPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Show(string content)
    {
        text.text = content;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
