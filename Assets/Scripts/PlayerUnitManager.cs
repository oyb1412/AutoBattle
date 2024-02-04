using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUnitManager : UnitManager
{


    [HideInInspector]public int buyUnitIndex;

    //������ ������ ǥ�����ִ� ������Ʈ ������
    public GameObject levelStarPrefabs;



    //�� ���� ǥ���ϴ� �̹��� ������Ʈ
    [SerializeField]Image[] levelStar;

    //�� ����Ʈ
    public ParticleSystem healEffect; 
    
    //���� ����Ʈ
    public ParticleSystem blessEffect;

    //���� �����ΰ�?
    protected bool isBless;

    //�÷��̾� ������ �������� ����
    public GameObject thisPrefab;

    //�������� ǥ���� �̹���
    [SerializeField] Image[] itemSlotImageObject;

    //������ ��������Ʈ
    [SerializeField] Sprite[] itemImages;

    //������ ���� ����
    public bool[] isItem;

    //���̾ ���� ����
    public bool isFireRing;

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
    }
    private void Start()
    {

    }
    override protected void Update()
    {
        if(transform.position.x > GameManager.instance.playerUnitController.rimitPos[1] && GameManager.instance.levelManager.currentState == StateType.BATTLE)
        {
            base.MoveToTarget(LevelManager.enemyLayer);
        }
        base.Update();
        ShowLevelStar();
    }

    protected void ShowLevelStar()
    {
        saveLevelStar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z));
    }

    /// <summary>
    /// ���� �ռ� �� ��ȭ
    /// </summary>
    public void UnitLevelUp()
    {
        level++;
        moveSpeed *= 1.05f;
        attackSpeed *= 1.5f;
        attackDamage *= 1.5f;
        attackRange *= 1.5f;
        transform.localScale *= 1.3f;
        if(level == 2)
            levelStar[1].gameObject.SetActive(true);
        else if(level == 3)
        {
            levelStar[0].gameObject.SetActive(false);
            levelStar[1].gameObject.SetActive(false);
            levelStar[2].gameObject.SetActive(true);
        }
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
            isItem[itemNum] = true;

            //ȹ���� �������� ����� �÷��̾� �ɷ�ġ ���
            switch(itemNum)
            {
                case (int)itemType.SWORD:
                    attackDamage *= 1.3f;
                    break;
                case (int)itemType.BOW:
                    attackSpeed *= 0.7f;
                    break;
                case (int)itemType.AROMR:
                    maxHP *= 1.3f;
                    currentHP = maxHP;
                    break;
                case (int)itemType.SHOES:
                    moveSpeed *= 1.3f;
                    break;
                case (int)itemType.HEALTHRING:
                    maxHP *= 1.3f;
                    currentHP = maxHP;
                    break;
                case (int)itemType.FIRERING:
                    isFireRing = true;
                    break;
                case (int)itemType.ABSOLUTERING:
                    attackDamage *= 1.3f;
                    attackSpeed *= 0.7f;
                    moveSpeed *= 1.3f;
                    maxHP *= 1.3f;
                    currentHP = maxHP;
                    break;
            }
            return true;

        }
        else if(itemSlotImageObject[1].sprite == null)
        {
            itemSlotImageObject[1].sprite = itemImages[itemNum];
            itemSlotImageObject[1].color = Color.white;
            isItem[itemNum] = true;
            return true;
        }
        else if(itemSlotImageObject[2].sprite == null)
        {
            itemSlotImageObject[2].sprite = itemImages[itemNum];
            itemSlotImageObject[2].color = Color.white;
            isItem[itemNum] = true;
            return true;
        }
        else
        {
            //��� ������ ������ ��á���Ƿ� �������� ���� �� ����
            Debug.Log("���Բ���");
            return false;
        }
    }
}
