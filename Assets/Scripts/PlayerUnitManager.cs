using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerUnitManager : UnitManager
{


    [HideInInspector]public int buyUnitIndex;

    //유닛의 레벨을 표시해주는 오브젝트 프리펩
    public GameObject levelStarPrefabs;

    public float synageDamage;
    public float synageAttackSpeed;
    public float synageMoveSpeed;
    public float synageHP; 
    
    public float itemDamage;
    public float itemAttackSpeed;
    public float itemMoveSpeed;
    public float itemHP;

    //각 별을 표시하는 이미지 오브젝트
    [SerializeField]Image[] levelStar;

    //힐 이펙트
    public ParticleSystem healEffect; 
    
    //블레스 이펙트
    public ParticleSystem blessEffect;

    //소환,승리시 위치 재조정 시 이펙트
    public ParticleSystem summonEffect;

    //블레스 상태인가?
    protected bool isBless;

    //아이템을 표시할 이미지
    [SerializeField] Image[] itemSlotImageObject;

    //아이템 스프라이트
    [SerializeField] Sprite[] itemImages;

    //아이템 착용 여부
    public bool[] isItem;

    //파이어링 공격 주기 체크
    public bool isFireRing;

    //착용하고있는 아이템의 종류
    public int[] itemNum;
    protected override void Awake()
    {
        base.Awake();
        saveLevelStar = Instantiate(levelStarPrefabs, GameObject.Find("OverrayCanvas").transform);
        level = 1;
 
        levelStar = new Image[3];
        isItem = new bool[itemImages.Length];
        levelStar = saveLevelStar.GetComponentsInChildren<Image>();
        saveLevelStar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z));
        for (int i = 1; i < levelStar.Length; i++)
        {
            levelStar[i].gameObject.SetActive(false);
        }
        itemSlotImageObject = new Image[3];
        itemSlotImageObject[0] = saveSlider.GetComponentsInChildren<Image>()[4];
        itemSlotImageObject[1] = saveSlider.GetComponentsInChildren<Image>()[6];
        itemSlotImageObject[2] = saveSlider.GetComponentsInChildren<Image>()[8];

        itemNum = new int[itemSlotImageObject.Length];
        for (int i = 0; i < itemNum.Length; i++)
        {
            itemNum[i] = 10;
        }
    }
    private void Start()
    {
        itemDamage = saveAttackDamage * 0.3f;
        itemAttackSpeed = saveAttackSpeed * 0.3f;
        itemMoveSpeed = saveMoveSpeed * 0.3f;
        itemHP = saveMaxHp * 0.3f;
    }

    /// <summary>
    /// 일정 주기마다 자신의 공격 데미지 / 5의 데미지를 주변 1.5범위에 입힘
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator SetFireRing(float time)
    {
        isFireRing = true;
        var obj = Physics2D.CircleCastAll(transform.position, 1.5f, Vector2.zero, 0f, LayerMask.GetMask("EnemyUnit"));
        if(obj.Length > 0)
        {
            for(int i = 0; i <obj.Length; i++)
            {
                obj[i].transform.GetComponent<EnemyUnitManager>().SetHP(-attackDamage * 0.2f);
            }
        }
        yield return new WaitForSeconds(time);
        isFireRing = false;
    }
    override protected void Update()
    {
        if (currentUnitState == unitState.DIE)
            return;

        if (transform.position.x > GameManager.instance.playerUnitController.rimitPos[1] && GameManager.instance.levelManager.currentState == StateType.BATTLE)
        {
            base.MoveToTarget(LevelManager.enemyLayer);
            if (isItem[5] && !isFireRing)
            {
                StartCoroutine(SetFireRing(1f));
            }
        }
        base.Update();
        ShowLevelStar();
    }

    protected void ShowLevelStar()
    {
        if(saveLevelStar)
        saveLevelStar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z));
    }

    /// <summary>
    /// 유닛 합성 시 변화
    /// </summary>
    public void UnitLevelUp()
    {
        level++;
        moveSpeed *= 1.3f;
        saveMoveSpeed *= 1.3f;
        attackSpeed *= 0.8f;
        saveAttackSpeed *= 0.8f;
        attackDamage *= 1.3f;
        saveAttackDamage *= 1.3f;
        attackRange *= 1.3f;
        saveAttackRange *= 1.3f;
        transform.localScale *= 1.1f;
        if(level == 2)
            levelStar[1].gameObject.SetActive(true);
        else if(level == 3)
        {
            levelStar[0].gameObject.SetActive(false);
            levelStar[1].gameObject.SetActive(false);
            levelStar[2].gameObject.SetActive(true);
        }

        if (playerUnitType == playerUnitType.MELEE1)
            count++;
    }



    /// <summary>
    /// 게임 승리시 플레이어 리셋을 위한 정보
    /// </summary>

    public void ResetObject(Vector3 pos, float hp)
    {
        transform.position = pos;
        currentHP = hp;
    }

    /// <summary>
    /// 아이템 착용 
    /// </summary>
    /// <param name="itemNum">착용할 아이템</param>
    public bool SetItem(int itemNum)
    {
        //비어있는 가장 앞쪽 슬롯에 아이템 배치
        if(itemSlotImageObject[0].sprite == null)
        {
            itemSlotImageObject[0].sprite = itemImages[itemNum];
            itemSlotImageObject[0].color = Color.white;
            this.itemNum[0] = itemNum;
            isItem[itemNum] = true;

            //획득한 아이템을 모토로 플레이어 능력치 상승
            switch(itemNum)
            {
                case (int)itemType.SWORD:
                    attackDamage += itemDamage;
                    saveAttackDamage += itemDamage;
                    break;
                case (int)itemType.BOW:
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    break;
                case (int)itemType.AROMR:
                    maxHP += itemHP;
                    currentHP = maxHP;
                    
                    break;
                case (int)itemType.SHOES:
                    moveSpeed += itemMoveSpeed;
                    break;
                case (int)itemType.HEALTHRING:
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
                case (int)itemType.FIRERING:
                    isFireRing = true;
                    break;
                case (int)itemType.ABSOLUTERING:
                    attackDamage += itemAttackSpeed;
                    saveAttackDamage += itemDamage;
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    moveSpeed += itemMoveSpeed;
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
            }
            return true;

        }
        else if(itemSlotImageObject[1].sprite == null)
        {
            itemSlotImageObject[1].sprite = itemImages[itemNum];
            itemSlotImageObject[1].color = Color.white;
            this.itemNum[1] = itemNum;
            isItem[itemNum] = true;

            //획득한 아이템을 모토로 플레이어 능력치 상승
            switch (itemNum)
            {
                case (int)itemType.SWORD:
                    attackDamage += itemDamage;
                    saveAttackDamage += itemDamage;
                    break;
                case (int)itemType.BOW:
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    break;
                case (int)itemType.AROMR:
                    maxHP += itemHP;
                    currentHP = maxHP;

                    break;
                case (int)itemType.SHOES:
                    moveSpeed += itemMoveSpeed;
                    break;
                case (int)itemType.HEALTHRING:
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
                case (int)itemType.FIRERING:
                    isFireRing = true;
                    break;
                case (int)itemType.ABSOLUTERING:
                    attackDamage += itemAttackSpeed;
                    saveAttackDamage += itemDamage;
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    moveSpeed += itemMoveSpeed;
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
            }
            return true;
        }
        else if(itemSlotImageObject[2].sprite == null)
        {
            itemSlotImageObject[2].sprite = itemImages[itemNum];
            itemSlotImageObject[2].color = Color.white;
            this.itemNum[2] = itemNum;
            isItem[itemNum] = true;

            //획득한 아이템을 모토로 플레이어 능력치 상승
            switch (itemNum)
            {
                case (int)itemType.SWORD:
                    attackDamage += itemDamage;
                    saveAttackDamage += itemDamage;
                    break;
                case (int)itemType.BOW:
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    break;
                case (int)itemType.AROMR:
                    maxHP += itemHP;
                    currentHP = maxHP;

                    break;
                case (int)itemType.SHOES:
                    moveSpeed += itemMoveSpeed;
                    break;
                case (int)itemType.HEALTHRING:
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
                case (int)itemType.FIRERING:
                    isFireRing = true;
                    break;
                case (int)itemType.ABSOLUTERING:
                    attackDamage += itemAttackSpeed;
                    saveAttackDamage += itemDamage;
                    attackSpeed -= itemAttackSpeed;
                    saveAttackSpeed -= itemAttackSpeed;
                    moveSpeed += itemMoveSpeed;
                    maxHP += itemHP;
                    currentHP = maxHP;
                    break;
            }
            return true;
        }
        else
        {
            //모든 아이템 슬롯이 꽉찼으므로 아이템을 먹을 수 없음
            Debug.Log("슬롯꽉참");
            return false;
        }
    }
    /// <summary>
    /// 장착중인 모든 아이템 해제
    /// 판매 or 합성할때 실행
    /// </summary>
    public void ClearItem()
    {
        for(int i = 0;i<itemSlotImageObject.Length;i++)
        {
            //아이템 슬롯 i번에 아이템이 들어있는 경우
            if (itemSlotImageObject[i].sprite != null)
            {
                //착용중인 아이템 정보를 토대로 아이템 생성
                var item = Instantiate(GameManager.instance.itemPrefabs[itemNum[i]], null);

                //아이템이 도달할 위치를 전장 내로 랜덤하게 계산
                var rimitpos = GameManager.instance.playerUnitController.rimitPos;
                var ranx = Random.Range(rimitpos[1], rimitpos[0]);
                var rany = Random.Range(rimitpos[2], rimitpos[3]);

                //아이템을 랜덤 위치로 1.5초에 걸쳐 이동
                item.transform.DOMove(new Vector2(ranx, rany), 1.5f);

                //해제한 아이템을 모토로 플레이어 능력치 하락
                switch (itemNum[i])
                {
                    case (int)itemType.SWORD:
                        attackDamage -= itemDamage;
                        saveAttackDamage -= itemDamage;
                        break;
                    case (int)itemType.BOW:
                        attackSpeed += itemAttackSpeed;
                        saveAttackSpeed += itemAttackSpeed;
                        break;
                    case (int)itemType.AROMR:
                        maxHP -= itemHP;
                        currentHP = maxHP;
                        break;
                    case (int)itemType.SHOES:
                        moveSpeed -= itemMoveSpeed;
                        break;
                    case (int)itemType.HEALTHRING:
                        maxHP -= itemHP;

                        currentHP = maxHP;
                        break;
                    case (int)itemType.FIRERING:
                        isFireRing = false;
                        break;
                    case (int)itemType.ABSOLUTERING:
                        attackDamage -= itemDamage;
                        saveAttackDamage -= itemDamage;
                        attackSpeed += itemAttackSpeed;
                        saveAttackSpeed += itemAttackSpeed;
                        moveSpeed -= itemMoveSpeed;
                        maxHP -= itemHP;
                        currentHP = maxHP;
                        break;
                }

                //초기화된 스탯을 기반으로 시너지를 재조정
                GameManager.instance.playerUnitController.SynageSet(0);
                //실제 유닛의 아이템 해제
                itemSlotImageObject[i].sprite = null;
                itemSlotImageObject[i].color = new Color(1f, 1f, 1f, 0f);
                isItem[itemNum[i]] = false;
                itemNum[i] = 10;
            }
        }
    }
    
}
