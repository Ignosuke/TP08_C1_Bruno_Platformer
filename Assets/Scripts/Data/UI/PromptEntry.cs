using System;
using UnityEngine;

// Un texto de ayuda con su id. Las teclas van como iconos con <sprite name="J">
[Serializable]
public class PromptEntry
{
    [SerializeField] private string id;
    [TextArea]
    [SerializeField] private string text;

    // Para que Unity pueda crear uno vacio desde el Inspector
    public PromptEntry() { }

    public PromptEntry(string id, string text)
    {
        this.id = id;
        this.text = text;
    }

    public string GetId() => id;
    public string GetText() => text;
}
