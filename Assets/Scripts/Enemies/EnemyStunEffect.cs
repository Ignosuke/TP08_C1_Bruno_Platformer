using UnityEngine;

// Estrellitas que giran arriba de la cabeza mientras el enemigo esta aturdido.
// Pregunta al cerebro cada frame, asi se apagan solas al terminar el aturdimiento, al morir o al reaparecer
public class EnemyStunEffect : MonoBehaviour
{
    [SerializeField] private EnemyBrain brain;
    [SerializeField] private Health health;
    [Tooltip("Child renderer above the head. It is not flipped, so the stars never mirror")]
    [SerializeField] private SpriteRenderer stars;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesPerSecond = 12f;

    private float timer;

    private void OnEnable()
    {
        stars.enabled = false;
    }

    private void Update()
    {
        bool isStunned = brain.GetIsStunned() && !health.GetIsDead();
        if (!isStunned)
        {
            stars.enabled = false;
            return;
        }

        // Arranca desde el primer frame cada vez que aparece
        if (!stars.enabled) timer = 0f;
        stars.enabled = true;

        timer += Time.deltaTime;
        stars.sprite = frames[(int)(timer * framesPerSecond) % frames.Length];
    }
}
