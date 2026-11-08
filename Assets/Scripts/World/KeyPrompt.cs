using UnityEngine;

// Zona de tutorial. Mientras el jugador esta adentro el cartel del HUD muestra las teclas y el texto
[RequireComponent(typeof(BoxCollider2D))]
public class KeyPrompt : MonoBehaviour
{
    [SerializeField] private PromptSo promptData;
    [SerializeField] private string promptId;
    [SerializeField] private PromptPanel panel;

    // Se reconoce al jugador por su componente, no por tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMover>() != null) panel.Show(promptData.GetText(promptId));
    }

    // Al descargar la escena el cartel puede destruirse antes que la zona
    private void OnTriggerExit2D(Collider2D other)
    {
        if (panel != null && other.GetComponent<PlayerMover>() != null) panel.Hide();
    }
}
