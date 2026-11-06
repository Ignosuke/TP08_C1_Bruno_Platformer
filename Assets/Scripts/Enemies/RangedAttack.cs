using UnityEngine;
using UnityEngine.Pool;

// Dispara una flecha en el frame del golpe. Si el mover esta esquivando espera, asi primero se aleja y despues dispara
public class RangedAttack : EnemyAttack
{
    // Las flechas se reciclan en vez de crearse y destruirse en cada disparo, asi no se genera basura ni tirones
    private ObjectPool<Arrow> arrowPool;

    protected override void Awake()
    {
        base.Awake();
        arrowPool = new ObjectPool<Arrow>(CreateArrow, GetArrow, ReleaseArrow, DestroyArrow);
    }

    // Las flechas sueltas no son hijas del arquero, las guardadas se destruyen con el
    private void OnDestroy()
    {
        arrowPool.Clear();
        arrowPool = null;
    }

    protected override bool IsTargetInRange()
    {
        return sight.CanSeePlayer() && Mathf.Abs(sight.GetPlayerDistanceX()) <= enemyData.GetAttackRange() && !mover.GetIsEvading();
    }

    protected override void PerformHit()
    {
        int facing = mover.GetDirection();
        Vector2 offset = enemyData.GetArrowSpawnOffset();
        Vector3 spawn = transform.position + new Vector3(offset.x * facing, offset.y, 0f);

        Arrow arrow = arrowPool.Get();
        arrow.Launch(enemyData, GetCurrentAttack(), facing, spawn, new Vector2(transform.position.x, spawn.y), ReturnArrow);
    }

    // Si el arquero ya no existe la flecha que seguia volando se destruye sola
    private void ReturnArrow(Arrow arrow)
    {
        if (arrowPool != null) arrowPool.Release(arrow);
        else Destroy(arrow.gameObject);
    }

    private Arrow CreateArrow() => Instantiate(enemyData.GetArrowPrefab());
    private static void GetArrow(Arrow arrow) => arrow.gameObject.SetActive(true);
    private static void ReleaseArrow(Arrow arrow) => arrow.gameObject.SetActive(false);

    // Al cerrar la escena la flecha puede haberse destruido antes que el arquero
    private static void DestroyArrow(Arrow arrow)
    {
        if (arrow != null) Destroy(arrow.gameObject);
    }
}
