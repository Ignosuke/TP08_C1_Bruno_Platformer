using UnityEngine;

// El sonido de click de la interfaz de la escena. Vive aparte de los botones porque al cerrar un panel
// el boton se apaga y cortaria el sonido
[RequireComponent(typeof(AudioSource))]
public class UiClickPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>Suena una vez, los clicks seguidos se pisan sin cortarse. El pitch es de la fuente entera, por eso se pone siempre</summary>
    public void Play(float pitch = 1f)
    {
        if (clickClip == null) return;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clickClip);
    }
}
