using TMPro;
using UnityEngine;

// Pone en el texto la entrada del PromptSo, asi los paneles fijos no guardan el texto en la escena
[RequireComponent(typeof(TMP_Text))]
public class PromptText : MonoBehaviour
{
    [SerializeField] private PromptSo promptData;
    [SerializeField] private string promptId;

    private void Awake()
    {
        GetComponent<TMP_Text>().text = promptData.GetText(promptId);
    }
}
