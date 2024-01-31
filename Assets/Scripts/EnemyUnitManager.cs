using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitManager : UnitManager
{
    [Header("UnitStatus")]
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
        if(dir != Vector2.zero)
        {
            base.SetAttackCol(ref enemyUnitStatus, dir, enemyUnitStatus.attackRange);
            base.SetAnmation(enemyUnitStatus, transform);
            base.Update();
        }

    }

    /// <summary>
    /// 유닛 본체에서 공격에 대한 충돌판정 진행
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(LevelManager.playerMeleeAttackTag))
        {
            var attack = collision.transform.parent.transform.parent.GetComponent<PlayerUnitManager>();

            base.SetHP(attack.playerUnitStatus.attackDamage,
                enemyUnitStatus);
        }
        if (collision.CompareTag(LevelManager.bulletTag))
        {
            Debug.Log("1");
            var bullet = collision.GetComponent<BulletController>();
            if (bullet.GroupType == groupType.PLAYER)
            {
                base.SetHP(-bullet.bulletDamage, enemyUnitStatus);
                Destroy(bullet.gameObject);
            }
        }
    }
}
