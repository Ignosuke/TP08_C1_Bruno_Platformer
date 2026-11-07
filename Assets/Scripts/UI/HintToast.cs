using System.Collections;
using TMPro;
using UnityEngine;

// Aviso corto en el HUD que se desvanece solo, al activar un checkpoint o comprar en la tienda
[RequireComponent(typeof(CanvasGroup))]
public class HintToast : MonoBehaviour
{
    [SerializeField] private PromptSo promptData;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private Shop shop;
    [Tooltip("Real seconds fully visible")]
    [SerializeField] private float showTime = 2f;
    [Tooltip("Real seconds of the fade out")]
    [SerializeField] private float fadeTime = 0.5f;

    private CanvasGroup group;
    private Coroutine showing;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    private void OnEnable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated += OnCheckpointActivated;
        shop.OnPurchased += OnPurchased;
    }

    private void OnDisable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated -= OnCheckpointActivated;
        shop.OnPurchased -= OnPurchased;
    }

    private void OnCheckpointActivated()
    {
        Show("Checkpoint");
    }

    private void OnPurchased()
    {
        Show("Purchase");
    }

    // Un aviso nuevo reemplaza al anterior
    public void Show(string promptId)
    {
        text.text = promptData.GetText(promptId);
        if (showing != null) StopCoroutine(showing);
        showing = StartCoroutine(ShowThenFade());
    }

    // Tiempo real, asi el hitstop no estira el aviso
    private IEnumerator ShowThenFade()
    {
        group.alpha = 1f;
        yield return new WaitForSecondsRealtime(showTime);

        float fadeCounter = fadeTime;
        while (fadeCounter > 0f)
        {
            fadeCounter -= Time.unscaledDeltaTime;
            group.alpha = Mathf.Clamp01(fadeCounter / fadeTime);
            yield return null;
        }
        group.alpha = 0f;
    }
}
