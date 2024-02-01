using UnityEngine;

public enum UnitSmallType { MELEE0, MELEE1, MELEE2, RANGE0, RANGE1, RANGE2, MAGE0, MAGE1, MAGE2 };
public class PlayerUnitManager : UnitManager
{
    [Header("UnitStatus")]
    public UnitStatus playerUnitStatus;
    [HideInInspector]public enum UpDownStatus { UP, DOWN };
    [HideInInspector]public UpDownStatus updownStatus;
    [HideInInspector]public int buyUnitIndex;
    public UnitSmallType unitSmallType;
    private void Start()
    {

        playerUnitStatus.currentHP = playerUnitStatus.maxHP;
    }
    override protected void Update()
    {

        if (LevelManager.instance.currentState == StateType.NONBATTLE)
            return;

        var dir = base.MoveToTarget(transform,LevelManager.enemyLayer,ref playerUnitStatus);
        if(dir != Vector2.zero)
        {
            base.SetAttackCol(ref playerUnitStatus, dir, playerUnitStatus.attackRange);
            base.SetAnmation(playerUnitStatus, transform);
            base.Update();
        }
    }

    /// <summary>
    /// 유닛 본체에서 공격에 대한 충돌판정 진행
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(LevelManager.enemyMeleeAttackTag))
        {
            var attack = collision.transform.parent.transform.parent.GetComponent<EnemyUnitManager>();
            base.SetHP(attack.enemyUnitStatus.attackDamage, playerUnitStatus);
        }
        if(collision.CompareTag(LevelManager.bulletTag))
        {
            var bullet = collision.GetComponent<BulletController>();
            if(bullet.GroupType == groupType.ENEMY)
            {
                base.SetHP(-bullet.bulletDamage, playerUnitStatus);
                Destroy(bullet);
            }
        }
    }

    /// <summary>
    /// 유닛 합성 시 변화
    /// </summary>
    public void UnitLevelUp()
    {
        playerUnitStatus.level++;
        playerUnitStatus.moveSpeed *= 1.05f;
        playerUnitStatus.attackSpeed *= 1.5f;
        playerUnitStatus.attackDamage *= 1.5f;
        playerUnitStatus.attackRange *= 1.5f;
        transform.localScale *= 1.3f;
    }
}
