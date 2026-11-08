using UnityEngine;
using UnityEngine.Audio;

// El mixer arranca con sus valores de fabrica en cada escena, esto le pone los volumenes guardados
public class VolumeLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [Tooltip("Exposed parameters of the mixer")]
    [SerializeField] private string[] parameters = { "MasterVolume", "MusicVolume", "SfxVolume" };

    // En Start y no en Awake, en Awake el mixer todavia ignora SetFloat
    private void Start()
    {
        foreach (string parameter in parameters) VolumeSettings.Apply(mixer, parameter);
    }
}
