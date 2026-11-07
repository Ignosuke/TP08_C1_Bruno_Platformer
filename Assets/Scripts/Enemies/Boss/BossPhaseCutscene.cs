using System.Collections;
using UnityEngine;

// El cambio de fase. Cuando se acaba la vida de la primera fase congela al jugador, entran las franjas negras,
// la camara mira al jefe, grita, aparece la espada en llamas, la barra se vuelve a llenar sola y al final se suelta todo.
// El jefe espera un rato quieto antes de volver a pelear, asi el jugador tiene tiempo de reaccionar
[RequireComponent(typeof(BossPhase), typeof(EnemyHealth), typeof(EnemySight))]
public class BossPhaseCutscene : MonoBehaviour
{
    [SerializeField] private BossSo bossData;
    [Tooltip("Letterbox bars on the HUD, assigned by the builder")]
    [SerializeField] private LetterboxBars letterbox;
    [SerializeField] private CameraFocusChannel cameraFocusChannel;

    private BossPhase phase;
    private EnemyHealth health;
    private EnemyAttack attack;
    private Rigidbody2D playerBody;
    private Behaviour[] playerControls = new Behaviour[0];
    private Coroutine routine;

    private void Awake()
    {
        phase = GetComponent<BossPhase>();
        health = GetComponent<EnemyHealth>();
        attack = GetComponent<EnemyAttack>();

        // Los mismos controles que apaga PlayerRespawner al morir
        PlayerHealth player = GetComponent<EnemySight>().GetPlayer();
        if (player == null) return;
        playerBody = player.GetComponent<Rigidbody2D>();
        playerControls = new Behaviour[]
        {
            player.GetComponent<PlayerMover>(), player.GetComponent<PlayerJumper>(), player.GetComponent<PlayerDasher>(),
            player.GetComponent<PlayerAttacker>(), player.GetComponent<PlayerDefender>(), player.GetComponent<PlayerHealer>(),
        };
    }

    private void OnEnable()
    {
        health.OnHealthChanged += CheckPhaseEnd;
    }

    // Si se apaga en medio (reinicio por reaparicion) suelta al jugador igual
    private void OnDisable()
    {
        health.OnHealthChanged -= CheckPhaseEnd;
        if (routine == null) return;

        routine = null;
        Release();
        phase.EndRecovery();
    }

    private void CheckPhaseEnd(int current, int max)
    {
        if (phase.GetIsPhaseTwo() || phase.GetIsChanging() || routine != null) return;
        if (current > 0 && current <= phase.GetPhaseTwoHealth()) routine = StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        attack.Cancel();
        phase.StartChange();
        SetPlayerFrozen(true);
        if (letterbox != null) letterbox.Show();
        cameraFocusChannel.Focus(transform);

        yield return new WaitForSeconds(bossData.GetShoutDuration());
        phase.EnterPhaseTwo();      // La barra ve la fase nueva y se llena sola

        yield return new WaitForSeconds(bossData.GetRegenDuration());
        Release();

        yield return new WaitForSeconds(bossData.GetPhaseTwoStartDelay());
        routine = null;
        phase.EndRecovery();
    }

    private void Release()
    {
        phase.EndChange();
        SetPlayerFrozen(false);
        if (letterbox != null) letterbox.Hide();
        cameraFocusChannel.Clear();
    }

    // Quieto en x, si estaba en el aire termina de caer
    private void SetPlayerFrozen(bool isFrozen)
    {
        foreach (Behaviour control in playerControls)
        {
            if (control != null) control.enabled = !isFrozen;
        }
        if (isFrozen && playerBody != null) playerBody.linearVelocity = new Vector2(0f, playerBody.linearVelocity.y);
    }
}
