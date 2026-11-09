using System.Collections;
using TMPro;
using UnityEngine;

// Aviso corto en el HUD que se desvanece solo, al activar un checkpoint, comprar en la tienda
// o recibir el primer golpe de la partida, para enseñar la cura
[RequireComponent(typeof(CanvasGroup))]
public class HintToast : MonoBehaviour
{
    [SerializeField] private PromptSo promptData;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private Shop shop;
    [Tooltip("The first hit of the run shows how to heal")]
    [SerializeField] private PlayerHealth playerHealth;
    [Tooltip("The heal hint stays until the player starts healing")]
    [SerializeField] private PlayerHealer playerHealer;
    [Tooltip("Real seconds fully visible")]
    [SerializeField] private float showTime = 2f;
    [Tooltip("Real seconds of the fade out")]
    [SerializeField] private float fadeTime = 0.5f;

    private CanvasGroup group;
    private Coroutine showing;
    private bool hasShownHeal;
    private bool isWaitingForHeal;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    private void OnEnable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated += OnCheckpointActivated;
        shop.OnPurchased += OnPurchased;
        if (playerHealth != null) playerHealth.OnDamaged += OnPlayerDamaged;
        if (playerHealer != null) playerHealer.OnHealStarted += OnHealStarted;
    }

    private void OnDisable()
    {
        foreach (Checkpoint checkpoint in checkpoints) checkpoint.OnActivated -= OnCheckpointActivated;
        shop.OnPurchased -= OnPurchased;
        if (playerHealth != null) playerHealth.OnDamaged -= OnPlayerDamaged;
        if (playerHealer != null) playerHealer.OnHealStarted -= OnHealStarted;
    }

    private void OnCheckpointActivated()
    {
        Show("Checkpoint");
    }

    private void OnPurchased()
    {
        Show("Purchase");
    }

    // Solo una vez por partida
    private void OnPlayerDamaged(DamageInfo info)
    {
        if (hasShownHeal) return;
        hasShownHeal = true;
        Show(PromptSo.FirstHurtId);
        isWaitingForHeal = playerHealer != null;
    }

    private void OnHealStarted()
    {
        isWaitingForHeal = false;
    }

    // Un aviso nuevo reemplaza al anterior
    public void Show(string promptId)
    {
        text.text = promptData.GetText(promptId);
        isWaitingForHeal = false;
        if (showing != null) StopCoroutine(showing);
        showing = StartCoroutine(ShowThenFade());
    }

    // Tiempo real, asi el hitstop no estira el aviso
    private IEnumerator ShowThenFade()
    {
        group.alpha = 1f;
        yield return new WaitForSecondsRealtime(showTime);
        while (isWaitingForHeal) yield return null;     // El de la cura sigue hasta que se cura

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
