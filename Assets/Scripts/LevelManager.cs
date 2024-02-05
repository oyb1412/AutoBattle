using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEditor.Progress;
//���� ������Ʈ ������ �迭
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE, WAIT, SELECT}
public class LevelManager : MonoBehaviour
{
    public StateType currentState;

    //���� string���ڿ��� ����� ����
    public const string playerLayer = "PlayerUnit";
    public const string enemyLayer = "EnemyUnit";
    public const string enemyTag = "Enemy";
    public const string playerTag = "Player";
    public const string enemyMeleeAttackTag = "EnemyMeleeAttack";
    public const string playerMeleeAttackTag = "PlayerMeleeAttack";
    public const string bulletTag = "Bullet";
    public const string SpawnColliderLayer = "SpawnCollider";
    public const string SellBinTagNLayer = "SellBin";
    public const string SpawnColliderTag0 = "SpawnCollider0";
    public const string SpawnColliderTag1 = "SpawnCollider1";
    public const string SpawnColliderTag2 = "SpawnCollider2";
    public const string SpawnColliderTag3 = "SpawnCollider3";
    public const string SpawnColliderTag4 = "SpawnCollider4";

    [Header("NextStageGameObject")]
    [SerializeField] GameObject doNextStageBtn;
    [SerializeField] GameObject centerLine;
    [SerializeField] GameObject mainPanel;

    [Header("PlayerUnitStatus")]
    //���� �� ���
    public const int buyCost = 10;
    //�Ǹ� �� ���
    public const int sellCost = 5;
    //���� �� ���
    public const int reRullCost = 2;
    //���� �ִ� ����
    public const int maxLevel = 3;
    //���� �ռ� ���� ��
    public const int mixNum = 3;
    //���� ����
    [HideInInspector]public int currentRound;

    //���� ���� ���
    public int currentGold;


    [Header("UiObject")]
    [SerializeField] Text currentGoldText;
    [SerializeField] Text currentRoundText;

    //��ȯ�� �и� �ֳʹ� ���� ������
    [SerializeField] GameObject[] enemyMeleeUnitPrefabs;  
    //��ȯ�� ������ �ֳʹ� ���� ������
    [SerializeField] GameObject[] enemyRangeUnitPrefabs;  
    //��ȯ�� ��ũ �ֳʹ� ���� ������
    [SerializeField] GameObject[] enemyTankUnitPrefabs;

    //��ȯ�� ������
    Vector2[,] summonPos;

    //���� ������ ���� �����
    struct PlayerUnits
    {
        public float maxHp;
        public Vector2 pos;
    }

    //���� ������ ���� �����
    [SerializeField]PlayerUnits[] playerUnits;

    [SerializeField] PlayerUnitManager[] players;

    //������ ���� �ǳ�
    [SerializeField] GameObject selectPanel;

    //���� ������ �ǳ�
    [SerializeField] GameObject[] itemPanel;

    void Start()
    {
        currentState = StateType.NONBATTLE;
        doNextStageBtn.SetActive(true);
        currentRound = 1;
        currentRoundText.text = currentRound + " ����";
        currentGoldText.text = currentGold + " ���";
        summonPos = new Vector2[4, 5];
        for(int i = 0;i<5; i++)
        {
            for(int j = 0;j<4;j++)
            {
                summonPos[j, i] = new Vector2(5+(j * 2f), 5 - (i * 2f));
            }
        }

        for(int i = 0; i<itemPanel.Length; i++)
        {
            itemPanel[i].gameObject.SetActive(false);
        }
        SummonEnemyUnit(currentRound);

    }


    /// <summary>
    /// ��Ʋ �����϶�, ������ ���� �Ǵ��ؼ� ��Ʋ ���¸� ����.
    /// ������ ���������� ����
    /// </summary>
    public void ExitBattleState(string tag)
    {
        if(currentState == StateType.BATTLE) 
        {
            var target = GameObject.FindGameObjectsWithTag(tag);
            if(target.Length == 1)
            {
                //���� �й�
                if (target[0].GetComponent<PlayerUnitManager>() != null)
                {
                }
                //���� �¸�
                else if (target[0].GetComponent<EnemyUnitManager>() != null)
                {
                    StartCoroutine(RoundWinCorutine(3f));
                }

            }
        }
    }

    /// <summary>
    /// ���� �¸� �� �����ð�
    /// </summary>
    /// <param name="exittime"></param>
    /// <returns></returns>
    IEnumerator RoundWinCorutine(float exittime)
    {
        var time = new WaitForSeconds(exittime);
        SetPlayerUnit();
        currentState = StateType.WIN;
        centerLine.transform.DOMoveY(0f, exittime);

        yield return time;
        currentState = StateType.SELECT;
        SetSelectItem();
        currentRound++;
        currentRoundText.text = currentRound + " ����";
        SummonEnemyUnit(currentRound);
        centerLine.SetActive(true);
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void SelectItemClick(int index)
    {
        for (int i = 0; i < itemPanel.Length; i++)
        {
            itemPanel[i].gameObject.SetActive(false);
        }
        currentState = StateType.NONBATTLE;
        mainPanel.GetComponent<RectTransform>().DOAnchorPosY(-430f, 3f);
        doNextStageBtn.GetComponent<RectTransform>().DOAnchorPosY(-760f, 3f);
        if (index == 6)
        {
            SetGold(50);
        }
        else
            CreateItem(index);

        SetGold(30);
    }

    /// <summary>
    /// Ư�� ������ ����
    /// </summary>
    public void CreateItem(int index)
    {
        //�������� ������ ������ ���� ������ ����
        var item = Instantiate(GameManager.instance.itemPrefabs[index], null);
        //�������� ������ ��ġ�� ���� ���� �����ϰ� ���
        var rimitpos = GameManager.instance.playerUnitController.rimitPos;
        var ranx = Random.Range(rimitpos[1], rimitpos[0]);
        var rany = Random.Range(rimitpos[2], rimitpos[3]);
        //�������� ���� ��ġ�� 1.5�ʿ� ���� �̵�
        item.transform.DOMove(new Vector2(ranx, rany), 1.5f);
    }

    /// <summary>
    /// ���� ������ �˾�
    /// </summary>
    void SetSelectItem()
    {
        for (int i = 0; i < itemPanel.Length; i++)
        {
            itemPanel[i].gameObject.SetActive(false);
        }
        int[] ran = new int[3];

        while (true)
        {
            ran[0] = Random.Range(0, itemPanel.Length);
            ran[1] = Random.Range(0, itemPanel.Length);
            ran[2] = Random.Range(0, itemPanel.Length);

            if (ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2])
                break;
        }
        itemPanel[ran[0]].gameObject.SetActive(true);
        itemPanel[ran[1]].gameObject.SetActive(true);
        itemPanel[ran[2]].gameObject.SetActive(true);
    }
    void SummonEnemyUnit(int round)
    {
        switch (round)
        {
            case 1:
                Instantiate(enemyMeleeUnitPrefabs[0], summonPos[0, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[0], summonPos[0, 4], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[0], summonPos[2, 3], Quaternion.identity);
                break;
            case 2:
                
                break;

            case 3:
        
                break;
        }
    }
    /// <summary>
    /// ���������� ���� �� ���
    /// </summary>
    /// <param name="exittime"></param>
    /// <returns></returns>
    IEnumerator GoBattleStateCorutine(float exittime)
    {
        var time = new WaitForSeconds(exittime);
        currentState = StateType.WAIT;
        doNextStageBtn.GetComponent<RectTransform>().DOAnchorPosY(-1150f, exittime);
        centerLine.transform.DOMoveY(15f, exittime);
        mainPanel.GetComponent<RectTransform>().DOAnchorPosY(-720f, exittime);
        SavePlayerUnit();
        yield return time;
        currentState = StateType.BATTLE;
    }

    /// <summary>
    /// ���� ���� �� ��� �÷��̾� ������ ��ġ�� �ڱ��ڽ� �������� ����
    /// </summary>
    void SavePlayerUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag(playerTag);
        //playerUnits = new PlayerUnits[obj.Length];
        //for (int i = 0; i < obj.Length; i++)
        //{
        //    //unit����ü�� �⺻ ����
        //    playerUnits[i].pos = obj[i].transform.position;
        //    playerUnits[i].maxHp = obj[i].GetComponent<PlayerUnitManager>().maxHP;
        //}
        players = new PlayerUnitManager[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            //unit����ü�� �⺻ ����
            players[i] = obj[i].GetComponent<PlayerUnitManager>();
        }
    }

    /// <summary>
    /// ���� ���� �� �����ص� �÷��̾� ������ ��ġ�� ü���� ������� �÷��̾� ����
    /// </summary>
    void SetPlayerUnit()
    {
        //var obj = GameObject.FindGameObjectsWithTag(playerTag);
        //for(int i = 0;i < obj.Length; i++)
        //{
        //    obj[i].GetComponent<PlayerUnitManager>().ResetObject(playerUnits[i].pos, playerUnits[i].maxHp);
        //    obj[i].GetComponent<PlayerUnitManager>().summonEffect.Play();
        //    obj[i].GetComponent<PlayerUnitManager>().currentUnitState = unitState.JUMP;
        //}
        for(int i = 0;i<players.Length;i++)
        {
            Instantiate(GameManager.instance.playerUnitPrefabs[(int)players[i].playerUnitType], players[i].transform.position, Quaternion.identity);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
    }
    /// <summary>
    /// Start��ư Ŭ������ �ߵ�
    /// ������Ʈ�� ����
    /// </summary>
    public void NextStageBtnClick()
    {
        if(GameManager.instance.playerUnitController.currentActiveUnitNum == 0)
        {
            //��ġ�� ������ ������ ���� ���� �Ұ� ���
        }
        else if (currentState == StateType.NONBATTLE)
        {
            StartCoroutine(GoBattleStateCorutine(3f));
        }
    }

    public void SetGold(int value)
    {
        currentGold += value;
        currentGoldText.text = currentGold + " ���";
    }
}
