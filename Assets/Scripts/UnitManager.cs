using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum groupType { PLAYER, ENEMY}
public enum unitType { MELEE,RANGE,MAGE}
public enum unitState { MOVE,ATTACK,DIE,IDLE}
public class UnitManager : MonoBehaviour
{
    [System.Serializable]
    public class UnitStatus
    {
        [SerializeField]public unitType currentUnitType;
        [SerializeField]public unitState currentUnitState;
        [SerializeField]public groupType currentUnitGroup;
        [SerializeField]public float currentHP;
        [SerializeField]public float maxHP;
        [SerializeField]public float moveSpeed;
        [SerializeField]public float attackSpeed;
        [SerializeField]public float attackDamage;
        [SerializeField]public float attackRange;
        [SerializeField]public float minDistance;
        [SerializeField]public Collider2D attackCollider;
        [SerializeField]public GameObject attackBullet;
        [SerializeField]public GameObject levelStar;
        [SerializeField]public GameObject hpBar;

    }

    [SerializeField] protected RaycastHit2D[] targets;
    const float scanRange = 30f;
    const float dieSpeed = 0.5f;
    protected Collider2D unitCollider;
    protected Animator anime;
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
    /// 추적 대상에게 직접적으로 이동하는 함수
    /// </summary>
    /// <param name="trans">이동 할 유닛의 trans</param>
    /// <param name="layer">추적 대상의 layer</param>
    /// <param name="unit">이동할 유닛의 unitStatus</param>
    protected Vector2 MoveToTarget(Transform trans, string layer, ref UnitStatus unit)
    {
        var target = GetTarget(trans, layer);
        Vector2 dir = target.transform.position - transform.position;
        var sprite = GetComponent<SpriteRenderer>();
        if (dir.magnitude > unit.minDistance)
        {
            transform.Translate(dir.normalized * unit.moveSpeed * Time.deltaTime);
            unit.currentUnitState = unitState.MOVE;
        }
        else
            unit.currentUnitState = unitState.ATTACK;

        #region Flip반전
        if (unit.currentUnitGroup == groupType.PLAYER)
        {
            if(target.transform.position.x < trans.position.x)
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

    protected void SetAnmation(UnitStatus unit)
    {
        switch(unit.currentUnitState)
        {
            case unitState.MOVE:
                anime.SetBool("Run", true);
                break;
            case unitState.ATTACK:
                if(!animationTrigger)
                    StartCoroutine(MeleeAttackCorutine(unit));
                break;
            case unitState.DIE:
                if (!animationTrigger)
                    StartCoroutine(DieCorutine(unit));
                    break;
        }
    }

    /// <summary>
    /// 사망시 애니메이터를 사용하지 않고
    /// 직접 오브젝트 삭제
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
        Destroy(gameObject);
    }

    /// <summary>
    /// attackSpeed에 한번씩만 공격 트리거 작동
    /// 실제 attackCol 활성화는 애니메이션에서 진행
    /// </summary>
    IEnumerator MeleeAttackCorutine(UnitStatus unit)
    {
        var attacktime = new WaitForSeconds(unit.attackSpeed);
        animationTrigger = true;
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");    
        yield return attacktime;
        animationTrigger = false;

    }
    protected void SetAttackCol(ref UnitStatus unit, Vector2 dir, float range)
    {
        unit.attackCollider.offset = dir.normalized * range;
    }
    /// <summary>
    /// 가장 가까운 적 추적
    /// </summary>
    /// <param name="trans">이 함수를 실행할 유닛의 trans</param>
    /// <param name="layer">추적대상 유닛의 Layer</param>
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
        return result.GetComponent<UnitManager>();
    }


    /// <summary>
    /// 유닛의 hp 조정
    /// 사망시 남은 유닛의 수 체크
    /// 남은 유닛이 없으면 스테이트 변경
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="layer"></param>
    protected void SetHP(float value, UnitStatus unit, string layer)
    {
        unit.currentHP += value;
        if (unit.currentHP <= 0)
        {
            LevelManager.instance.ExitBattleState(layer);
        }
        else
        {
            unitCollider.enabled = false;
            //사망판정
        }
    }
}
