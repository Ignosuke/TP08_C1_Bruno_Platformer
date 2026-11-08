using UnityEngine;
using UnityEngine.Audio;

// Los volumenes guardados en PlayerPrefs, de 0 a 1. La clave es el nombre del parametro expuesto del mixer
public static class VolumeSettings
{
    private const float MinDecibels = -80f;     // Silencio en el mixer
    private const float DefaultVolume = 0.5f;

    public static float GetVolume(string parameter) => PlayerPrefs.GetFloat(parameter, DefaultVolume);

    // Save hace falta en la web, ahi no hay cierre de la aplicacion que lo guarde
    public static void SetVolume(AudioMixer mixer, string parameter, float volume)
    {
        PlayerPrefs.SetFloat(parameter, volume);
        PlayerPrefs.Save();
        Apply(mixer, parameter);
    }

    // El oido es logaritmico, por eso log10. La mitad del volumen son unos -6 dB y 0 es silencio
    public static void Apply(AudioMixer mixer, string parameter)
    {
        float volume = GetVolume(parameter);
        mixer.SetFloat(parameter, volume > 0f ? Mathf.Log10(volume) * 20f : MinDecibels);
    }
}
