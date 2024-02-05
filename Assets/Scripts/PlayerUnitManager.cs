using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerUnitManager : UnitManager
{


    [HideInInspector]public int buyUnitIndex;

    //������ ������ ǥ�����ִ� ������Ʈ ������
    public GameObject levelStarPrefabs;

    public float synageDamage;
    public float synageAttackSpeed;
    public float synageMoveSpeed;
    public float synageHP; 
    
    public float itemDamage;
    public float itemAttackSpeed;
    public float itemMoveSpeed;
    public float itemHP;

    //�� ���� ǥ���ϴ� �̹��� ������Ʈ
    [SerializeField]Image[] levelStar;

    //�� ����Ʈ
    public ParticleSystem healEffect; 
    
    //���� ����Ʈ
    public ParticleSystem blessEffect;

    //��ȯ,�¸��� ��ġ ������ �� ����Ʈ
    public ParticleSystem summonEffect;

    //���� �����ΰ�?
    protected bool isBless;

    //�������� ǥ���� �̹���
    [SerializeField] Image[] itemSlotImageObject;

    //������ ��������Ʈ
    [SerializeField] Sprite[] itemImages;

    //������ ���� ����
    public bool[] isItem;

    //���̾ ���� �ֱ� üũ
    public bool isFireRing;

    //�����ϰ��ִ� �������� ����
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
    /// ���� �ֱ⸶�� �ڽ��� ���� ������ / 5�� �������� �ֺ� 1.5������ ����
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
    /// ���� �ռ� �� ��ȭ
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
    /// ���� �¸��� �÷��̾� ������ ���� ����
    /// </summary>

    public void ResetObject(Vector3 pos, float hp)
    {
        transform.position = pos;
        currentHP = hp;
    }

    /// <summary>
    /// ������ ���� 
    /// </summary>
    /// <param name="itemNum">������ ������</param>
    public bool SetItem(int itemNum)
    {
        //����ִ� ���� ���� ���Կ� ������ ��ġ
        if(itemSlotImageObject[0].sprite == null)
        {
            itemSlotImageObject[0].sprite = itemImages[itemNum];
            itemSlotImageObject[0].color = Color.white;
            this.itemNum[0] = itemNum;
            isItem[itemNum] = true;

            //ȹ���� �������� ����� �÷��̾� �ɷ�ġ ���
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

            //ȹ���� �������� ����� �÷��̾� �ɷ�ġ ���
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

            //ȹ���� �������� ����� �÷��̾� �ɷ�ġ ���
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
            //��� ������ ������ ��á���Ƿ� �������� ���� �� ����
            Debug.Log("���Բ���");
            return false;
        }
    }
    /// <summary>
    /// �������� ��� ������ ����
    /// �Ǹ� or �ռ��Ҷ� ����
    /// </summary>
    public void ClearItem()
    {
        for(int i = 0;i<itemSlotImageObject.Length;i++)
        {
            //������ ���� i���� �������� ����ִ� ���
            if (itemSlotImageObject[i].sprite != null)
            {
                //�������� ������ ������ ���� ������ ����
                var item = Instantiate(GameManager.instance.itemPrefabs[itemNum[i]], null);

                //�������� ������ ��ġ�� ���� ���� �����ϰ� ���
                var rimitpos = GameManager.instance.playerUnitController.rimitPos;
                var ranx = Random.Range(rimitpos[1], rimitpos[0]);
                var rany = Random.Range(rimitpos[2], rimitpos[3]);

                //�������� ���� ��ġ�� 1.5�ʿ� ���� �̵�
                item.transform.DOMove(new Vector2(ranx, rany), 1.5f);

                //������ �������� ����� �÷��̾� �ɷ�ġ �϶�
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

                //�ʱ�ȭ�� ������ ������� �ó����� ������
                GameManager.instance.playerUnitController.SynageSet(0);
                //���� ������ ������ ����
                itemSlotImageObject[i].sprite = null;
                itemSlotImageObject[i].color = new Color(1f, 1f, 1f, 0f);
                isItem[itemNum[i]] = false;
                itemNum[i] = 10;
            }
        }
    }
    
}
