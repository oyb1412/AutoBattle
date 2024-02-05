using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.MaterialProperty;

//���� ������Ʈ ������ �迭
public enum unitState { MOVE,ATTACK,DIE,WAIT,JUMP}

public enum unitType { MELEE,RANGE,MAGE}
public enum unitGroup { PLAYER,ENEMY}
public enum playerUnitType { MELEE0, MELEE1, MELEE2, RANGE0, RANGE1, RANGE2, MAGE0, MAGE1, MAGE2 }

public class UnitManager : MonoBehaviour
{
   [Header("UnitStatus")]
   //������ ���� ����
   public unitState currentUnitState;

    //���� Ÿ��
    public playerUnitType playerUnitType;

    //������ ���� Ÿ��
    public unitType currentUnitType;

    //������ �Ǿ�
    public unitGroup currentUnitGroup;

   //������ ���� ü��
   public float currentHP;

   //������ �ִ� ü��
   public float maxHP;

   //������ �̵� �ӵ�
   public float moveSpeed;

   //������ ���� �ӵ�
   public float attackSpeed;

   //������ ���� �ӵ� ����
   public float saveAttackSpeed;

   //������ ���� ������
   public float attackDamage;

   //������ ���� ������ ����
   public float saveAttackDamage;

   //������ ���� ��Ÿ�
   public float attackRange;

   //������ ���� ��Ÿ� ����
   public float saveAttackRange;

    //������ �ִ� ü�� ����
    public float saveMaxHp;  

    //������ �̵� �ӵ� ����
   public float saveMoveSpeed;

    //������ ���� ���� Ƚ��
    public int count;

    //������ ����
    public int level;


    //������ ����� ������ ü�¹�
    protected Slider saveSlider;

    //ü�¹� ǥ�ø� ���� ���� ����� �ʵ�
    UnitManager player, enemy;

    //�ֺ� �� ����� �迭
    protected RaycastHit2D[] targets;

    //�ֺ� ���� ��ĵ�� �Ÿ�
    const float scanRange = 30f;

    //������� �� destroy���� �ɸ��� �ð�
    const float dieSpeed = 0.5f;

    //���� ��ü�� �ִϸ�����
    protected Animator anime;

    //���� ����� �� ����� ������Ʈ
    UnitManager target;

    //������ �߻�ü
    public GameObject attackBullet;

    //�߻�ü�� �߻�Ǵ� ��ġ ������Ʈ
    public GameObject firePos;




    //�и������� ���� ����
    public Collider2D attackCollider;

    //���� ��ü�� �ݶ��̴�
    protected Collider2D unitCollider;

    //���� �� �� �߸��� ����Ʈ
    public ParticleSystem dustEffect;

    //�и������� ���� ����Ʈ
    public ParticleSystem attackEffect;

    //������ ü�¹� ������
    public Slider hpSlider;

    //������ ������ ǥ�����ִ� ������Ʈ
    protected GameObject saveLevelStar;

    //���� üũ
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
    /// ������ ���� ���¿� ���� �ִϸ��̼� ����
    /// ����� �ѹ��� �����ϱ� ���� Ʈ���Ÿ� ���
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
    /// attackSpeed�� �ѹ����� ���� Ʈ���� �۵�
    /// ���� attackCol Ȱ��ȭ�� �ִϸ��̼ǿ��� ����
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

            //�������� ������ 2 �̻��̰�, ������ ���
            if (level > 1 && playerUnitType == playerUnitType.RANGE0)
            {
                //������ ����� ��Ƽ�� Ȯ�� ����
                var ran = Random.Range(0, (5 - player.level));
                if (ran == 0)
                {
                    //��Ƽ�� �ߵ��� �ణ�� �����ð� �Ŀ� �ѹ� �� �߻�
                    StartCoroutine(MultiShotCorutine(0.2f));
                }
            }
        }
        yield return attacktime;
        isAttack = false;
    }


    /// <summary>
    /// ������ ���� ���� �ڷ�ƾ
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
            //���� Ÿ���� ������1�϶�
            if (playerUnitType == playerUnitType.MAGE1)
            {
                //���� ���� ���� �Ʊ��� ����
                var target = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0f, LayerMask.GetMask("PlayerUnit"));
                if (target.Length > 0)
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        var player = target[i].transform.GetComponent<PlayerUnitManager>();
                        float critical = 0;
                        //������ 2 �̻��� ��
                        if (level > 1)
                        {
                            //���� ����
                            critical = level * 3;
                        }
                        //�Ʊ��� ü�� ����
                        player.currentHP += attackDamage + critical;
                        //�Ʊ� ü������ ����Ʈ �÷���
                        player.healEffect.Play();
                    }
                }
            }
            //���� Ÿ���� ������0�϶�
            else if (playerUnitType == playerUnitType.MAGE0)
            {
                //���� ���� ���� �Ʊ��� ����
                var target = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0f, LayerMask.GetMask("PlayerUnit"));
                if (target.Length > 0)
                {
                    for (int i = 0; i < target.Length; i++)
                    {
                        var player = target[i].transform.GetComponent<PlayerUnitManager>();
                        float bless = attackDamage;
                        //�Ʊ��� ���ݷ�dmf ���� �ð���ŭ ��
                        StartCoroutine(BlessCorutine(2f, player, attackDamage / 2f));
                    }
                }
            }
            //���� Ÿ���� ������ 2�϶�
            else if (playerUnitType == playerUnitType.MAGE2)
            {
                //������ 2�̻��϶�
                if (level > 1)
                {
                    var ran = Random.Range(0, 4 - level);
                    if (ran == 0)
                    {
                        //����ü �߰�
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
            //������ 2 �̻��� ���
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
    /// ����� �ִϸ����͸� ������� �ʰ�
    /// ���� ������Ʈ ����
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
    /// ���� ����Ʈ �ִϸ��̼ǿ��� ����
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
    /// �и������� �������� ���� �ݶ��̴� ��ġ
    /// </summary>
    protected void SetAttackCol(Vector2 dir)
    {
        if (attackCollider != null)
            attackCollider.transform.localPosition = (Vector3)dir;
        if(attackEffect != null)
            attackEffect.transform.localPosition = (Vector3)dir;
    }

    /// <summary>
    /// ������ hp ����
    /// ����� ���� ������ �� üũ
    /// ���� ������ ������ ������Ʈ ����
    /// ü�¹�, ������Ÿ ����
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
    /// ���� ��󿡰� ���������� �̵��ϴ� �Լ�
    /// </summary>
    /// <param name="layer">���� ����� layer</param>
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
            #region Flip����
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
    /// ���� ����� �� ����
    /// </summary>
    /// <param name="trans">�� �Լ��� ������ ������ trans</param>
    /// <param name="layer">������� ������ Layer</param>
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
    /// ������ ����ϰų� ���� �Ҹ�� ������ ������Ʈ ����
    /// </summary>
    public void DeleteOtherObject()
    {
        if (saveLevelStar)
            Destroy(saveLevelStar.gameObject);
        if (saveSlider)
            Destroy(saveSlider.gameObject);
    }


}
