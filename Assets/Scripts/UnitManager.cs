using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.MaterialProperty;

//현재 스테이트 구별용 배열
public enum unitState { MOVE,ATTACK,DIE,WAIT,JUMP}

public enum unitType { MELEE,RANGE,MAGE}
public enum unitGroup { PLAYER,ENEMY}
public enum playerUnitType { MELEE0, MELEE1, MELEE2, RANGE0, RANGE1, RANGE2, MAGE0, MAGE1, MAGE2 }

public class UnitManager : MonoBehaviour
{
   [Header("UnitStatus")]
   //유닛의 현재 상태
   public unitState currentUnitState;

    //유닛 타입
    public playerUnitType playerUnitType;

    //유닛의 공격 타입
    public unitType currentUnitType;

    //유닛의 피아
    public unitGroup currentUnitGroup;

   //유닛의 현재 체력
   public float currentHP;

   //유닛의 최대 체력
   public float maxHP;

   //유닛의 이동 속도
   public float moveSpeed;

   //유닛의 공격 속도
   public float attackSpeed;

   //유닛의 공격 속도 저장
   public float saveAttackSpeed;

   //유닛의 공격 데미지
   public float attackDamage;

   //유닛의 공격 데미지 저장
   public float saveAttackDamage;

   //유닛의 공격 사거리
   public float attackRange;

   //유닛의 공격 사거리 저장
   public float saveAttackRange;

    //유닛의 최대 체력 저장
    public float saveMaxHp;  

    //유닛의 이동 속도 저장
   public float saveMoveSpeed;

    //유닛의 공격 관통 횟수
    public int count;

    //유닛의 레벨
    public int level;


    //실제로 사용할 유닛의 체력바
    protected Slider saveSlider;

    //체력바 표시를 위한 정보 저장용 필드
    UnitManager player, enemy;

    //주변 적 저장용 배열
    protected RaycastHit2D[] targets;

    //주변 적을 스캔할 거리
    const float scanRange = 30f;

    //사망판정 후 destroy까지 걸리는 시간
    const float dieSpeed = 0.5f;

    //유닛 본체의 애니메이터
    protected Animator anime;

    //가장 가까운 적 저장용 오브젝트
    UnitManager target;

    //유닛의 발사체
    public GameObject attackBullet;

    //발사체가 발사되는 위치 오브젝트
    public GameObject firePos;




    //밀리유닛의 공격 판정
    public Collider2D attackCollider;

    //유닛 본체의 콜라이더
    protected Collider2D unitCollider;

    //유닛 발 밑 발먼지 이펙트
    public ParticleSystem dustEffect;

    //밀리유닛의 공격 이펙트
    public ParticleSystem attackEffect;

    //유닛의 체력바 프리펩
    public Slider hpSlider;

    //유닛의 레벨을 표시해주는 오브젝트
    protected GameObject saveLevelStar;

    //상태 체크
    public bool isDie;
    public bool isMove;
    public bool isJump;
    public bool isAttack;
    public bool isStun;


    virtual protected void Awake()
    {
        unitCollider = GetComponent<Collider2D>();
        anime = GetComponent<Animator>();
        saveSlider = Instantiate(hpSlider, GameObject.Find("OverrayCanvas").transform);
        saveAttackDamage = attackDamage;
        saveAttackSpeed = attackSpeed;
        saveAttackRange = attackRange;
        saveMaxHp = maxHP;
        saveMoveSpeed = moveSpeed;
        currentHP = maxHP;
    }
    virtual protected void Update()
    {
        if (GameManager.instance.levelManager.currentState == StateType.WAIT || GameManager.instance.levelManager.currentState == StateType.WIN)
        {
            currentUnitState = unitState.JUMP;

        }
        else
            isJump = false;

        //if(GameManager.instance.levelManager.currentState == StateType.NONBATTLE)
        //{
        //    currentUnitState = unitState.WAIT;
        //}
        anime.SetBool("Run", isMove);
        anime.SetBool("Jump", isJump);
        ShowHpSlider();

        if (!isStun)
            SetAnmation();
    }
    protected void ShowHpSlider()
    {
        if (currentUnitGroup == unitGroup.PLAYER)
        {
            if (GameManager.instance.MouseRayCast( "Player", "PlayerUnit"))
            {
                player = GameManager.instance.MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();
                player.saveSlider.gameObject.SetActive(true);
                player.saveSlider.value = player.currentHP / player.maxHP;
                player.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1.4f, player.transform.position.z));
            }
            else if (player != null)
            {
                if(player.saveSlider != null)
                player.saveSlider.gameObject.SetActive(false);
            }
        }
        else
        {
            if (GameManager.instance.MouseRayCast("Enemy", "EnemyUnit") != null)
            {
                enemy = GameManager.instance.MouseRayCast("Enemy", "EnemyUnit").GetComponent<EnemyUnitManager>();
                enemy.saveSlider.gameObject.SetActive(true);
                enemy.saveSlider.value = enemy.currentHP / enemy.maxHP;
                enemy.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.4f, enemy.transform.position.z)); 
            }
            else if (enemy != null)
            {
                if(enemy.saveSlider != null)
                    enemy.saveSlider.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 유닛의 현재 상태에 따라 애니메이션 진행
    /// 사망은 한번만 실행하기 위해 트리거를 사용
    /// </summary>
    public void SetAnmation()
    {
        switch (currentUnitState)
        {
            case unitState.MOVE:
                if (!isMove)
                {
                    dustEffect.Play();
                    isMove = true;
                    isAttack = false;
                    isJump = false;
                }
                break;
            //case unitState.DIE:
            //    if (!isDie)
            //    {
            //        dustEffect.Stop();
            //        isAttack = false;
            //        isMove = false;
            //        isJump = false;
            //        Debug.Log("1");
            //        StartCoroutine(DieCorutine());
            //    }
            //    break;
            case unitState.ATTACK:
                if(!isAttack && target != null)
                {
                    dustEffect.Stop();
                    isMove = false;
                    isJump = false;
                    if (currentUnitType == unitType.MELEE)
                        StartCoroutine(MeleeAttackCorutine());
                    else if (currentUnitType == unitType.RANGE)
                        StartCoroutine(RangeAttackCorutine());
                    else if (currentUnitType == unitType.MAGE)
                        StartCoroutine(MageAttackCorutine());
                }
                break;
            case unitState.JUMP:
                if(!isJump)
                {
                    isJump = true;
                    dustEffect.Stop();
                }
                break;
        }
    }

    /// <summary>
    /// attackSpeed에 한번씩만 공격 트리거 작동
    /// 실제 attackCol 활성화는 애니메이션에서 진행
    /// </summary>
    IEnumerator MeleeAttackCorutine()
    {
        var attacktime = new WaitForSeconds(attackSpeed);
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        isAttack = true;
        yield return attacktime;
        isAttack = false;
    }

    IEnumerator RangeAttackCorutine()
    {
        var attacktime = new WaitForSeconds(attackSpeed);
        var shottime = new WaitForSeconds(attackSpeed * 0.4f);
        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        isAttack = true;
        yield return shottime;
        if (target == null)
            StopCoroutine(RangeAttackCorutine());
        else
        {
            var dir = target.transform.position - firePos.transform.position;
            var arrow = Instantiate(attackBullet, firePos.transform);
            if (currentUnitGroup == unitGroup.PLAYER)
                arrow.GetComponent<BulletController>().Init(target.transform.position, dir, attackDamage, unitGroup.PLAYER, level, playerUnitType);
            else
                arrow.GetComponent<BulletController>().Init(target.transform.position, dir, attackDamage, unitGroup.ENEMY);

            //레인저의 레벨이 2 이상이고, 헌터일 경우
            if (level > 1 && playerUnitType == playerUnitType.RANGE0)
            {
                //레벨에 비례해 멀티샷 확률 조정
                var ran = Random.Range(0, (5 - player.level));
                if (ran == 0)
                {
                    //멀티샷 발동시 약간의 지연시간 후에 한발 더 발사
                    StartCoroutine(MultiShotCorutine(0.2f));
                }
            }
        }
        yield return attacktime;
        isAttack = false;
    }


    /// <summary>
    /// 메이지 유닛 공격 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator MageAttackCorutine()
    {
        var attacktime = new WaitForSeconds(attackSpeed);
        var shottime = new WaitForSeconds(attackSpeed * 0.1f);

        anime.SetBool("Run", false);
        anime.SetTrigger("Attack");
        isAttack = true;
        yield return shottime;

        if(target != null)
        {
            var dir = target.transform.position - firePos.transform.position;
            var arrow = Instantiate(attackBullet, firePos.transform);
            //유닛 타입이 메이지1일때
            if (playerUnitType == playerUnitType.MAGE1)
            {
                //공격 범위 내의 아군을 조사
                var target = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0f, LayerMask.GetMask("PlayerUnit"));
                if (target.Length > 0)
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        var player = target[i].transform.GetComponent<PlayerUnitManager>();
                        float critical = 0;
                        //레벨이 2 이상일 때
                        if (level > 1)
                        {
                            //힐량 증가
                            critical = level * 3;
                        }
                        //아군의 체력 증가
                        player.currentHP += attackDamage + critical;
                        //아군 체력증가 이펙트 플레이
                        player.healEffect.Play();
                    }
                }
            }
            //유닛 타입이 메이지0일때
            else if (playerUnitType == playerUnitType.MAGE0)
            {
                //공격 범위 내의 아군을 조사
                var target = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0f, LayerMask.GetMask("PlayerUnit"));
                if (target.Length > 0)
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        var player = target[i].transform.GetComponent<PlayerUnitManager>();
                        float bless = attackDamage;
                        //아군의 공격력dmf 지정 시간만큼 업
                        StartCoroutine(BlessCorutine(2f, player, attackDamage / 2f));
                    }
                }
            }
            //유닛 타입이 메이지 2일때
            else if (playerUnitType == playerUnitType.MAGE2)
            {
                //레벨이 2이상일때
                if (level > 1)
                {
                    var ran = Random.Range(0, 4 - level);
                    if (ran == 0)
                    {
                        //투사체 추가
                        var dir1 = target.transform.position - firePos.transform.position;
                        var arrow1 = Instantiate(attackBullet, firePos.transform);
                        dir1 = new Vector2(dir1.x, dir1.y + 1f);
                        Debug.Log("1");
                        arrow1.GetComponent<BulletController>().Init(target.transform.position, dir1, attackDamage, unitGroup.PLAYER);
                    }
                }


            }
            arrow.GetComponent<BulletController>().Init(target.transform.position, dir, attackDamage, unitGroup.PLAYER);

        }
        yield return attacktime;
        isAttack =false;
    }

    IEnumerator BlessCorutine(float time, PlayerUnitManager unit, float damage)
    {
        if (unit)
        {
            unit.attackDamage += damage;
            //레벨이 2 이상인 경우
            if (level > 1)
                unit.attackSpeed -= 0.1f * level;

            unit.blessEffect.Play();
            yield return new WaitForSeconds(time);
            unit.attackDamage -= damage;
            if (level > 1)
                unit.attackSpeed += 0.1f * level;
            if(unit.blessEffect)
            unit.blessEffect.Stop();
        }
    }

    IEnumerator MultiShotCorutine(float time)
    {
        yield return new WaitForSeconds(time);
        var dir = target.transform.position - firePos.transform.position;
        var arrow = Instantiate(attackBullet, firePos.transform);
        arrow.GetComponent<BulletController>().Init(target.transform.position, dir, attackDamage, unitGroup.PLAYER);
    }

    /// <summary>
    /// 사망시 애니메이터를 사용하지 않고
    /// 직접 오브젝트 삭제
    /// </summary>
    IEnumerator DieCorutine()
    {
        var dietime = new WaitForSeconds(dieSpeed);
        Debug.Log("2");

        unitCollider.enabled = false;
        isDie = true;
        anime.SetBool("Run", false);
        anime.SetTrigger("Die");
        yield return dietime;
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
        {
            if (currentUnitGroup == unitGroup.ENEMY)
                GameManager.instance.levelManager.ExitBattleState(LevelManager.enemyTag);
            else
                GameManager.instance.levelManager.ExitBattleState(LevelManager.playerTag);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 공격 이펙트 애니메이션에서 실행
    /// </summary>
    public void SetAttackEffect()
    {
        if (attackEffect != null)
        {
            attackEffect.Play();
        }
        if(attackCollider.gameObject != null)
        {
            attackCollider.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 밀리유닛의 전방으로 어택 콜라이더 배치
    /// </summary>
    protected void SetAttackCol(Vector2 dir)
    {
        if (attackCollider != null)
            attackCollider.transform.localPosition = (Vector3)dir;
        if(attackEffect != null)
            attackEffect.transform.localPosition = (Vector3)dir;
    }

    /// <summary>
    /// 유닛의 hp 조정
    /// 사망시 남은 유닛의 수 체크
    /// 남은 유닛이 없으면 스테이트 변경
    /// 체력바, 레벨스타 삭제
    /// </summary>
    public void SetHP(float value)
    {
        currentHP += value;
        if (currentHP <= 0)
        {
            currentUnitState = unitState.DIE;
            DeleteOtherObject();
            dustEffect.Stop();
            isAttack = false;
            isMove = false;
            isJump = false;
            Debug.Log("1");
            StartCoroutine(DieCorutine());
        }
    }

    /// <summary>
    /// 추적 대상에게 직접적으로 이동하는 함수
    /// </summary>
    /// <param name="layer">추적 대상의 layer</param>
    protected void MoveToTarget(string layer)
    {
         target = GetTarget(layer);
        if(target != null && currentHP > 0)
        {
            Vector2 dir = target.transform.position - transform.position;
            var sprite = GetComponent<SpriteRenderer>();
            SetAttackCol(dir.normalized);
            if (dir.magnitude > attackRange)
            {
                transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
                currentUnitState = unitState.MOVE;

            }
            else
            {
                currentUnitState = unitState.ATTACK;
            }
            #region Flip반전
            if (currentUnitGroup == unitGroup.PLAYER)
            {
                if (target.transform.position.x < transform.position.x)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;

            }
            else
            {
                if (target.transform.position.x < transform.position.x)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;
            }

            #endregion
        }

    }



    /// <summary>
    /// 가장 가까운 적 추적
    /// </summary>
    /// <param name="trans">이 함수를 실행할 유닛의 trans</param>
    /// <param name="layer">추적대상 유닛의 Layer</param>
    /// <returns></returns>
    protected UnitManager GetTarget(string layer)
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, LayerMask.GetMask(layer));
       
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
    /// 유닛이 사망하거나 재료로 소모시 나머지 오브젝트 제거
    /// </summary>
    public void DeleteOtherObject()
    {
        if (saveLevelStar)
            Destroy(saveLevelStar.gameObject);
        if (saveSlider)
            Destroy(saveSlider.gameObject);
    }


}
