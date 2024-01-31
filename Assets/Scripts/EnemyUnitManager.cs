using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitManager : UnitManager
{
    public UnitStatus enemyUnitStatus;

    private void Start()
    {
        enemyUnitStatus.currentHP = enemyUnitStatus.maxHP;
    }
    override protected void Update()
    {
        if (LevelManager.instance.currentState == StateType.NONBATTLE)
            return;

        var dir = base.MoveToTarget(transform,LevelManager.playerLayer,ref enemyUnitStatus);
        base.SetAttackCol(ref enemyUnitStatus, dir, enemyUnitStatus.attackRange);
        base.SetAnmation(enemyUnitStatus);
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(LevelManager.playerMeleeAttackTag))
        {
            Debug.Log("1");
            base.SetHP(-collision.GetComponent<PlayerUnitManager>().playerUnitStatus.attackDamage,
                enemyUnitStatus, LevelManager.enemyLayer);
        }
    }
}
