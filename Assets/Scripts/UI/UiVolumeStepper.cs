using System;
using UnityEngine;
using UnityEngine.UI;

// Un volumen con menos, una barra y mas. Solo muestra el nivel y avisa el cambio, quien escucha lo aplica al mixer
public class UiVolumeStepper : MonoBehaviour
{
    private const int Steps = 10;

    [Tooltip("Exposed parameter of the mixer, also the PlayerPrefs key")]
    [SerializeField] private string parameter;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [Tooltip("Anchored to the left of the bar, its width shows the level")]
    [SerializeField] private RectTransform fill;

    private int step;

    public event Action<string, float> OnValueChanged;

    private void OnEnable()
    {
        minusButton.onClick.AddListener(Decrease);
        plusButton.onClick.AddListener(Increase);
    }

    private void OnDisable()
    {
        minusButton.onClick.RemoveListener(Decrease);
        plusButton.onClick.RemoveListener(Increase);
    }

    // Muestra un valor guardado sin avisar, asi abrir el panel no vuelve a guardar
    public void SetValue(float value)
    {
        step = Mathf.Clamp(Mathf.RoundToInt(value * Steps), 0, Steps);
        Refresh();
    }

    private void Decrease() => Change(-1);
    private void Increase() => Change(1);

    private void Change(int amount)
    {
        int next = Mathf.Clamp(step + amount, 0, Steps);
        if (next == step) return;

        step = next;
        Refresh();
        OnValueChanged?.Invoke(parameter, GetValue());
    }

    private void Refresh()
    {
        fill.anchorMax = new Vector2(GetValue(), 1f);
    }

    public float GetValue() => (float)step / Steps;
    public string GetParameter() => parameter;
}
