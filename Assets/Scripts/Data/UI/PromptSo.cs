using System.Collections.Generic;
using UnityEngine;

// Todos los textos de ayuda del juego, tutoriales, avisos y paneles de teclas. Las escenas solo guardan el id
[CreateAssetMenu(fileName = "PromptSo", menuName = "Data/Prompts")]
public class PromptSo : ScriptableObject
{
    // Tambien los usa el menu 15 para sumarlos a un PromptSo que ya existia
    public const string MenuHintText = "<sprite name=\"Up\"><sprite name=\"Down\"> Choose  <sprite name=\"J\"> Confirm  <sprite name=\"K\"> Back";
    public const string PauseHintText = "<sprite name=\"Up\"><sprite name=\"Down\"> Choose  <sprite name=\"J\"> Confirm  <sprite name=\"K\"><sprite name=\"Esc\"> Back";
    public const string CreditsText =
        "Developed by Ignacio Molina\n\n" +
        "Samurai, Oni, Archer, Executioner and Demon Samurai sprites\n" +
        "Spring and Autumn tilesets, sakura trees, props and butterflies\n" +
        "Forest backgrounds, Crimson Fantasy GUI, key icons\n\n" +
        "Font monogram by datagoblin";

    [SerializeField] private List<PromptEntry> entries = new List<PromptEntry>
    {
        new PromptEntry("Move", "<sprite name=\"A\"><sprite name=\"D\"> Move"),
        new PromptEntry("Jump", "<sprite name=\"Space\"> Jump"),
        new PromptEntry("Attack", "<sprite name=\"J\"> Attack"),
        new PromptEntry("Parry", "<sprite name=\"L\"> Defend, right before the hit to parry"),
        new PromptEntry("DoubleJump", "<sprite name=\"Space\"> again in the air to double jump"),
        new PromptEntry("Checkpoint", "Safe spot reached"),
        new PromptEntry("Purchase", "Bought"),
        new PromptEntry("ShopPrompt", "<sprite name=\"E\"> Shop"),
        new PromptEntry("ShopHint", "<sprite name=\"Up\"><sprite name=\"Down\"> Choose  <sprite name=\"J\"> Buy  <sprite name=\"K\"> Close"),
        new PromptEntry("MenuHint", MenuHintText),
        new PromptEntry("PauseHint", PauseHintText),
        new PromptEntry("Credits", CreditsText),
    };

    // Si falta el id se muestra el id, asi el error se ve en pantalla y no queda un panel vacio
    public string GetText(string id)
    {
        foreach (PromptEntry entry in entries)
        {
            if (entry.GetId() == id) return entry.GetText();
        }
        Debug.LogWarning("PromptSo has no entry '" + id + "'", this);
        return id;
    }
}
