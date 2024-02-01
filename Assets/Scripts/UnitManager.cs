using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.ParticleSystem;

//적아군 구별용 배열
public enum groupType { PLAYER, ENEMY}
//유닛 공격타입 구별용 배열
public enum unitType { MELEE,RANGE,MAGE}

//현재 스테이트 구별용 배열
public enum unitState { MOVE,ATTACK,DIE,IDLE}
public class UnitManager : MonoBehaviour
{
    //유닛 스텟 클래스
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


    //주변 적 저장용 배열
    protected RaycastHit2D[] targets;

    //주변 적을 스캔할 거리
    const float scanRange = 30f;

    //사망판정 후 destroy까지 걸리는 시간
    const float dieSpeed = 0.5f;

    //유닛 본체의 콜라이더
    protected Collider2D unitCollider;

    //유닛 본체의 애니메이터
    protected Animator anime;

    //가장 가까운 적 저장용 오브젝트
    UnitManager target;

    //단수 실행용 트리거
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
    /// 추적 대상에게 직접적으로 이동하는 함수
    /// </summary>
    /// <param name="trans">이동 할 유닛의 trans</param>
    /// <param name="layer">추적 대상의 layer</param>
    /// <param name="unit">이동할 유닛의 unitStatus</param>
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

            #region Flip반전
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
    /// 유닛의 현재 상태에 따라 애니메이션 진행
    /// 사망은 한번만 실행하기 위해 트리거를 사용
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
    /// 메이지 유닛 공격 코루틴
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
    /// 원거리 유닛 공격 코루틴
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
        if(unit.currentUnitGroup == groupType.ENEMY)
            LevelManager.instance.ExitBattleState(LevelManager.enemyTag);
        else
            LevelManager.instance.ExitBattleState(LevelManager.playerTag);

        Destroy(gameObject);

    }

    /// <summary>
    /// attackSpeed에 한번씩만 공격 트리거 작동
    /// 실제 attackCol 활성화는 애니메이션에서 진행
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
   /// 밀리유닛의 전방으로 어택 콜라이더 배치
   /// </summary>
    protected void SetAttackCol(ref UnitStatus unit, Vector2 dir, float range)
    {
        if(unit.attackCollider != null)
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
        if (targets.Length != 0)
            return result.GetComponent<UnitManager>();
        else
            return null;
    }


    /// <summary>
    /// 유닛의 hp 조정
    /// 사망시 남은 유닛의 수 체크
    /// 남은 유닛이 없으면 스테이트 변경
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
