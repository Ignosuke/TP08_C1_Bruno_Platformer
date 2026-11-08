using TMPro;
using UnityEngine;

// El record en el menu principal, lo guarda RunStats. Si nunca se termino una partida no se muestra
public class UiBestScore : MonoBehaviour
{
    [SerializeField] private string prefix = "Best ";
    [Tooltip("The text and its shadow copy")]
    [SerializeField] private TMP_Text[] texts;

    private void Start()
    {
        int best = RunStats.GetBestScore();
        foreach (TMP_Text text in texts) text.text = prefix + best;
        gameObject.SetActive(best > 0);
    }
}
