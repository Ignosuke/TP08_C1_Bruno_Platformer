using UnityEngine;

// El gong al activar un checkpoint y al ganar
[RequireComponent(typeof(AudioSource))]
public class GongPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip gongClip;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private RunStats runStats;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated += PlayGong;
        runStats.OnRunScored += PlayVictoryGong;
    }

    private void OnDisable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated -= PlayGong;
        runStats.OnRunScored -= PlayVictoryGong;
    }

    private void PlayVictoryGong(ScoreResult result) => PlayGong();

    private void PlayGong()
    {
        if (gongClip != null) audioSource.PlayOneShot(gongClip);
    }
}
