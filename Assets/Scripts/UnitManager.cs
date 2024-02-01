using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.ParticleSystem;

//���Ʊ� ������ �迭
public enum groupType { PLAYER, ENEMY}
//���� ����Ÿ�� ������ �迭
public enum unitType { MELEE,RANGE,MAGE}

//���� ������Ʈ ������ �迭
public enum unitState { MOVE,ATTACK,DIE,IDLE}
public class UnitManager : MonoBehaviour
{
    //���� ���� Ŭ����
    [System.Serializable]
    public class UnitStatus
    {
       public unitType currentUnitType;
       public unitState currentUnitState;
       public groupType currentUnitGroup;
       public float currentHP;
       public float maxHP;
       public float moveSpeed;
       public float attackSpeed;
       public float attackDamage;
       public float attackRange;
       public float minDistance;
       public int level;
       public Collider2D attackCollider;
       public GameObject attackBullet;
       public GameObject levelStar;
       public GameObject hpBar;
       public GameObject firePos;

    }


    //�ֺ� �� ����� �迭
    protected RaycastHit2D[] targets;

    //�ֺ� ���� ��ĵ�� �Ÿ�
    const float scanRange = 30f;

    //������� �� destroy���� �ɸ��� �ð�
    const float dieSpeed = 0.5f;

    //���� ��ü�� �ݶ��̴�
    protected Collider2D unitCollider;

    //���� ��ü�� �ִϸ�����
    protected Animator anime;

    //���� ����� �� ����� ������Ʈ
    UnitManager target;

    //�ܼ� ����� Ʈ����
    bool attackTrigger;
    bool animationTrigger;

    private void Awake()
    {
        unitCollider = GetComponent<Collider2D>();
        anime = GetComponent<Animator>();
    }

    virtual protected void Update()
    {

    }
    /// <summary>
    /// ���� ��󿡰� ���������� �̵��ϴ� �Լ�
    /// </summary>
    /// <param name="trans">�̵� �� ������ trans</param>
    /// <param name="layer">���� ����� layer</param>
    /// <param name="unit">�̵��� ������ unitStatus</param>
    protected Vector2 MoveToTarget(Transform trans, string layer, ref UnitStatus unit)
    {
        if(GetTarget(trans,layer) == null)
        {
            return Vector2.zero;
        }
        else
        {
            target = GetTarget(trans, layer);
            Vector2 dir = target.transform.position - transform.position;
            var sprite = GetComponent<SpriteRenderer>();
            if (dir.magnitude > unit.minDistance)
            {
                transform.Translate(dir.normalized * unit.moveSpeed * Time.deltaTime);
                unit.currentUnitState = unitState.MOVE;
            }
            else if(unit.currentHP > 0)
                unit.currentUnitState = unitState.ATTACK;

            #region Flip����
            if (unit.currentUnitGroup == groupType.PLAYER)
            {
                if (target.transform.position.x < trans.position.x)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;

            }
            else
            {
                if (target.transform.position.x < trans.position.x)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;
            }
            #endregion

            return dir;
        }
        
    }

    /// <summary>
    /// ������ ���� ���¿� ���� �ִϸ��̼� ����
    /// ����� �ѹ��� �����ϱ� ���� Ʈ���Ÿ� ���
    /// </summary>
    protected void SetAnmation(UnitStatus unit, Transform trans)
    {
        switch(unit.currentUnitState)
        {
            case unitState.MOVE:
                anime.SetBool("Run", true);
                break;
            case unitState.ATTACK:
                if (!attackTrigger)
                {
                    if (unit.currentUnitType == unitType.MELEE)
                        StartCoroutine(MeleeAttackCorutine(unit));
                    else if(unit.currentUnitType != unitType.RANGE)
                        StartCoroutine(RangeAttackCorutine(unit, trans));
                    else
                        StartCoroutine(MageAttackCorutine(unit, trans));

                }
                break;
            case unitState.DIE:
                if (!animationTrigger)
                    StartCoroutine(DieCorutine(unit));
                    break;
        }
    }

    /// <summary>
    /// ������ ���� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator MageAttackCorutine(UnitStatus unit, Transform trans)
    {
        var attacktime = new WaitForSeconds(unit.attackSpeed);
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        attackTrigger = true;

        yield return attacktime;
        var dir = target.transform.position - unit.firePos.transform.position;
        var arrow = Instantiate(unit.attackBullet, unit.firePos.transform);
        if (unit.currentUnitGroup == groupType.PLAYER)
            arrow.GetComponent<BulletController>().Init(target.transform.position, dir, unit.attackDamage, groupType.PLAYER);
        else
            arrow.GetComponent<BulletController>().Init(target.transform.position, dir, unit.attackDamage, groupType.ENEMY);
        attackTrigger = false;
    }

    /// <summary>
    /// ���Ÿ� ���� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator RangeAttackCorutine(UnitStatus unit, Transform trans)
    {
        var attacktime = new WaitForSeconds(unit.attackSpeed);
        var shottime = new WaitForSeconds(unit.attackSpeed * 0.4f);
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        attackTrigger = true;
        yield return shottime;
        var dir = target.transform.position - unit.firePos.transform.position;
        var arrow = Instantiate(unit.attackBullet, unit.firePos.transform);
        if(unit.currentUnitGroup == groupType.PLAYER)
            arrow.GetComponent<BulletController>().Init(target.transform.position,dir, unit.attackDamage, groupType.PLAYER);
        else
            arrow.GetComponent<BulletController>().Init(target.transform.position, dir, unit.attackDamage, groupType.ENEMY);
        yield return attacktime;
        attackTrigger = false;
    }

    /// <summary>
    /// ����� �ִϸ����͸� ������� �ʰ�
    /// ���� ������Ʈ ����
    /// </summary>
    IEnumerator DieCorutine(UnitStatus unit)
    {
        var dietime = new WaitForSeconds(dieSpeed);
        unit.attackCollider.enabled = false;
        unitCollider.enabled = false;
        animationTrigger = true;
        anime.SetBool("Run", false);
        anime.SetTrigger("Die");
        yield return dietime;
        animationTrigger = false;
        if(unit.currentUnitGroup == groupType.ENEMY)
            LevelManager.instance.ExitBattleState(LevelManager.enemyTag);
        else
            LevelManager.instance.ExitBattleState(LevelManager.playerTag);

        Destroy(gameObject);

    }

    /// <summary>
    /// attackSpeed�� �ѹ����� ���� Ʈ���� �۵�
    /// ���� attackCol Ȱ��ȭ�� �ִϸ��̼ǿ��� ����
    /// </summary>
    IEnumerator MeleeAttackCorutine(UnitStatus unit)
    {
        var attacktime = new WaitForSeconds(unit.attackSpeed);
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        attackTrigger = true;
        yield return attacktime;
        attackTrigger = false;
    }

   /// <summary>
   /// �и������� �������� ���� �ݶ��̴� ��ġ
   /// </summary>
    protected void SetAttackCol(ref UnitStatus unit, Vector2 dir, float range)
    {
        if(unit.attackCollider != null)
        unit.attackCollider.offset = dir.normalized * range;
    }

    /// <summary>
    /// ���� ����� �� ����
    /// </summary>
    /// <param name="trans">�� �Լ��� ������ ������ trans</param>
    /// <param name="layer">������� ������ Layer</param>
    /// <returns></returns>
    protected UnitManager GetTarget(Transform trans,string layer)
    {
        targets = Physics2D.CircleCastAll(trans.position, scanRange, Vector2.zero, 0, LayerMask.GetMask(layer));
       
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector2 myPos = transform.position;

            Vector2 targetPos = target.transform.position;

            float curDiff = Vector2.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;

                result = target.transform;
            }
        }
        if (targets.Length != 0)
            return result.GetComponent<UnitManager>();
        else
            return null;
    }


    /// <summary>
    /// ������ hp ����
    /// ����� ���� ������ �� üũ
    /// ���� ������ ������ ������Ʈ ����
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="layer"></param>
    protected void SetHP(float value, UnitStatus unit)
    {
        unit.currentHP += value;
        if (unit.currentHP <= 0)
        {
            unitCollider.enabled = false;
            unit.currentUnitState = unitState.DIE;
        }
        else
        {

        }
    }
}
