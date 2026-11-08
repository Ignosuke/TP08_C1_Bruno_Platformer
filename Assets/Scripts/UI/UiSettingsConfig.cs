using UnityEngine;
using UnityEngine.Audio;

// Une los steppers de volumen con el mixer. Al abrir muestra lo guardado, al cambiar uno lo aplica y lo guarda
public class UiSettingsConfig : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private UiVolumeStepper[] steppers;

    private void OnEnable()
    {
        foreach (UiVolumeStepper stepper in steppers)
        {
            stepper.SetValue(VolumeSettings.GetVolume(stepper.GetParameter()));
            stepper.OnValueChanged += Apply;
        }
    }

    private void OnDisable()
    {
        foreach (UiVolumeStepper stepper in steppers) stepper.OnValueChanged -= Apply;
    }

    private void Apply(string parameter, float volume)
    {
        VolumeSettings.SetVolume(mixer, parameter, volume);
    }
}
