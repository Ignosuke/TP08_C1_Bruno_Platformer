using UnityEngine;
using UnityEngine.UI;

// La barra del jefe en el HUD, muestra lo que queda de la fase actual.
// Con un golpe el relleno baja rapido y una franja clara se queda en el valor viejo, espera y baja despacio.
// Golpes seguidos la dejan en el valor mas alto hasta que baja. Al llenarse para la segunda fase
// la franja sube primero y el relleno la alcanza al final. Solo se ve durante la pelea
public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private BossBrain brain;
    [SerializeField] private Health health;
    [SerializeField] private BossPhase phase;
    [SerializeField] private BossSo bossData;
    [SerializeField] private GameObject container;
    [Tooltip("Filled image, the current health")]
    [SerializeField] private Image fill;
    [Tooltip("Filled image behind the fill, the light strip that drains or refills")]
    [SerializeField] private Image chip;

    [Header("Feedback")]
    [Tooltip("Seconds the fill takes to drop to the new value, whatever the hit")]
    [SerializeField] private float fillDropTime = 0.12f;
    [Tooltip("Seconds the light strip stays at the old value after the last hit")]
    [SerializeField] private float chipDelay = 0.5f;
    [Tooltip("Seconds the light strip takes to drain once it starts, whatever the amount")]
    [SerializeField] private float chipDrainTime = 0.45f;

    private float target;
    private float fillSpeed;
    private float chipSpeed;
    private float chipDelayCounter;

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        phase.OnPhaseChanged += Refresh;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
        phase.OnPhaseChanged -= Refresh;
    }

    // En Start porque la vida se crea en el Awake del jefe
    private void Start()
    {
        Refresh();
    }

    private void OnHealthChanged(int current, int max) => Refresh();

    private void Refresh()
    {
        target = phase.GetFraction(health.GetCurrent());
        if (target >= fill.fillAmount) return;

        // La franja no se mueve de donde estaba, solo vuelve a esperar
        fillSpeed = GetSpeed(fill.fillAmount - target, fillDropTime);
        chipSpeed = 0f;
        chipDelayCounter = chipDelay;
    }

    // Tiempo sin escala, asi el hitstop no la frena
    private void Update()
    {
        bool isVisible = brain.GetIsFighting() && !health.GetIsDead();
        container.SetActive(isVisible);

        // Escondida queda al dia sin animar, asi al aparecer no se ve un relleno viejo
        if (!isVisible)
        {
            fill.fillAmount = target;
            chip.fillAmount = target;
            return;
        }

        if (fill.fillAmount > target) fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, target, fillSpeed * Time.unscaledDeltaTime);

        if (chip.fillAmount > target) Drain();
        else if (chip.fillAmount < target) Regen();

        if (chip.fillAmount >= target && fill.fillAmount < target) fill.fillAmount = target;
    }

    private void Drain()
    {
        if (chipDelayCounter > 0f)
        {
            chipDelayCounter -= Time.unscaledDeltaTime;
            return;
        }

        // La velocidad se fija al empezar a bajar, asi un golpe chico y uno grande tardan lo mismo
        if (chipSpeed <= 0f) chipSpeed = GetSpeed(chip.fillAmount - target, chipDrainTime);
        chip.fillAmount = Mathf.MoveTowards(chip.fillAmount, target, chipSpeed * Time.unscaledDeltaTime);
    }

    private void Regen()
    {
        chip.fillAmount = Mathf.MoveTowards(chip.fillAmount, target, GetSpeed(1f, bossData.GetRegenDuration()) * Time.unscaledDeltaTime);
    }

    // Con 0 segundos llega enseguida
    private float GetSpeed(float amount, float time) => time > 0f ? amount / time : float.MaxValue;
}
