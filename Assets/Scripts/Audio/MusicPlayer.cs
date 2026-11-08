using System.Collections;
using UnityEngine;

// La musica de la escena, en loop. En el nivel cambia al tema del jefe cuando empieza la pelea,
// vuelve al del nivel si se reinicia y se apaga al vencerlo. Todo en tiempo real, asi la pausa no la corta
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer current;

    [SerializeField] private AudioClip startClip;
    [Tooltip("Survives the scene change and keeps playing if the next scene starts with the same track, like Splash to MainMenu")]
    [SerializeField] private bool keepAcrossScenes;
    [Tooltip("Played while fighting the boss, can be empty in scenes without a boss")]
    [SerializeField] private AudioClip bossClip;
    [Tooltip("Can be empty in scenes without a boss")]
    [SerializeField] private BossBrain boss;
    [Tooltip("Real seconds of each fade")]
    [SerializeField] private float fadeTime = 0.5f;

    private AudioSource audioSource;
    private AudioClip currentClip;
    private Coroutine changing;
    private float fullVolume;

    // Si ya suena el mismo tema de la escena anterior este se descarta, si no reemplaza al anterior.
    // Apagarlo antes de destruirlo evita que llegue a sonar en el Start
    private void Awake()
    {
        if (current != null && current.keepAcrossScenes && keepAcrossScenes && current.startClip == startClip)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }
        if (current != null) Destroy(current.gameObject);
        current = this;
        if (keepAcrossScenes) DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        fullVolume = audioSource.volume;
    }

    private void OnEnable()
    {
        if (boss == null) return;
        boss.OnFightStarted += PlayBoss;
        boss.OnFightReset += PlayStart;
        boss.OnDefeated += Stop;
    }

    private void OnDisable()
    {
        if (boss == null) return;
        boss.OnFightStarted -= PlayBoss;
        boss.OnFightReset -= PlayStart;
        boss.OnDefeated -= Stop;
    }

    // El primer tema entra directo, sin fundido
    private void Start()
    {
        currentClip = startClip;
        audioSource.clip = startClip;
        if (startClip != null) audioSource.Play();
    }

    private void PlayBoss() => Change(bossClip);
    private void PlayStart() => Change(startClip);
    private void Stop() => Change(null);

    private void Change(AudioClip clip)
    {
        if (clip == currentClip) return;
        currentClip = clip;
        if (changing != null) StopCoroutine(changing);
        changing = StartCoroutine(FadeToClip(clip));
    }

    private IEnumerator FadeToClip(AudioClip clip)
    {
        yield return Fade(audioSource.volume, 0f);
        audioSource.Stop();
        audioSource.clip = clip;
        if (clip == null) yield break;

        audioSource.Play();
        yield return Fade(0f, fullVolume);
    }

    private IEnumerator Fade(float from, float to)
    {
        float counter = 0f;
        while (counter < fadeTime)
        {
            counter += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(from, to, counter / fadeTime);
            yield return null;
        }
        audioSource.volume = to;
    }
}
