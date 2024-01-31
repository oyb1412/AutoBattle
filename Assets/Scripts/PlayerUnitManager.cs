using UnityEngine;

public class PlayerUnitManager : UnitManager
{
    public UnitStatus playerUnitStatus;
    [HideInInspector]public enum UpDownStatus { UP, DOWN };
    [HideInInspector]public UpDownStatus updownStatus;
    [HideInInspector]public int buyUnitIndex;
    public int buyCost;
    public int sellCost;
    private void Start()
    {
        playerUnitStatus.currentHP = playerUnitStatus.maxHP;
    }
    override protected void Update()
    {
        if (LevelManager.instance.currentState == StateType.NONBATTLE)
            return;

        var dir = base.MoveToTarget(transform,LevelManager.enemyLayer,ref playerUnitStatus);
        base.SetAttackCol(ref playerUnitStatus, dir, playerUnitStatus.attackRange);
        base.SetAnmation(playerUnitStatus);
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(LevelManager.enemyMeleeAttackTag))
        {
            Debug.Log("1");
            base.SetHP(-collision.GetComponent<EnemyUnitManager>().enemyUnitStatus.attackDamage,
                playerUnitStatus, LevelManager.playerLayer);
        }
    }
}
