using TMPro;
using UnityEngine;

// Una fila del puntaje, el nombre ya viene puesto y aca se llenan el dato y los puntos
[RequireComponent(typeof(UiPopIn))]
public class UiScoreRow : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text pointsText;
    [Tooltip("Assigned in the Inspector, the panel can call Hide before this Awake")]
    [SerializeField] private UiPopIn popIn;

    public void Hide()
    {
        popIn.Hide();
    }

    public void Show(string value, string points)
    {
        valueText.text = value;
        pointsText.text = points;
        popIn.Play();
    }
}
