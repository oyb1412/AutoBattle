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
        base.AllWaysPlayAnimation(enemyUnitStatus);
        if (GameManager.instance.levelManager.currentState == StateType.NONBATTLE || GameManager.instance.levelManager.currentState == StateType.WAIT)
            return;

        var dir = base.MoveToTarget(transform,LevelManager.playerLayer,ref enemyUnitStatus);
        if(dir != Vector2.zero)
        {
            base.SetAttackCol(ref enemyUnitStatus, dir, this);
            base.SetAnmation(enemyUnitStatus, transform);
            base.Update();
        }

    }

    /// <summary>
    /// ���� ��ü���� ���ݿ� ���� �浹���� ����
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(LevelManager.playerMeleeAttackTag))
        {
            var attack = collision.transform.parent.transform.parent.GetComponent<PlayerUnitManager>();

            base.SetHP(-attack.playerUnitStatus.attackDamage,
                enemyUnitStatus);
        }
        if (collision.CompareTag(LevelManager.bulletTag))
        {
            var bullet = collision.GetComponent<BulletController>();
            if (bullet.GroupType == groupType.PLAYER)
            {
                base.SetHP(-bullet.bulletDamage, enemyUnitStatus);
                Destroy(bullet.gameObject);
            }
        }
    }
}
