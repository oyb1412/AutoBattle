using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUnitManager : UnitManager
{


    [HideInInspector]public int buyUnitIndex;

    //유닛의 레벨을 표시해주는 오브젝트 프리펩
    public GameObject levelStarPrefabs;



    //각 별을 표시하는 이미지 오브젝트
    [SerializeField]Image[] levelStar;

    //힐 이펙트
    public ParticleSystem healEffect; 
    
    //블레스 이펙트
    public ParticleSystem blessEffect;

    //블레스 상태인가?
    protected bool isBless;

    //플레이어 본인의 프리펩을 저장
    public GameObject thisPrefab;

    //아이템을 표시할 이미지
    [SerializeField] Image[] itemSlotImageObject;

    //아이템 스프라이트
    [SerializeField] Sprite[] itemImages;

    //아이템 착용 여부
    public bool[] isItem;

    //파이어링 착용 여부
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
    /// 유닛 합성 시 변화
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
            isItem[itemNum] = true;

            //획득한 아이템을 모토로 플레이어 능력치 상승
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
            //모든 아이템 슬롯이 꽉찼으므로 아이템을 먹을 수 없음
            Debug.Log("슬롯꽉참");
            return false;
        }
    }
}
