using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
//���� ������Ʈ ������ �迭
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE, WAIT}
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


    //���� ������ ���� �����
    struct PlayerUnits
    {
        public float maxHp;
        public Vector2 pos;
    }

    [SerializeField]PlayerUnits[] playerUnits;

    void Start()
    {
        currentState = StateType.NONBATTLE;
        doNextStageBtn.SetActive(true);
        currentRound = 1;
        currentRoundText.text = currentRound + " ����";
        currentGoldText.text = currentGold + " ���";

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
        mainPanel.GetComponent<RectTransform>().DOAnchorPosY(-430f, exittime);
        doNextStageBtn.GetComponent<RectTransform>().DOAnchorPosY(-250f, exittime);
        yield return time;
        currentState = StateType.NONBATTLE;
        currentRound++;
        currentRoundText.text = currentRound + " ����";
        centerLine.SetActive(true);
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
        doNextStageBtn.GetComponent<RectTransform>().DOAnchorPosY(-600f, exittime);
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
        playerUnits = new PlayerUnits[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            //unit����ü�� �⺻ ����
            playerUnits[i].pos = obj[i].transform.position;
            playerUnits[i].maxHp = obj[i].GetComponent<PlayerUnitManager>().maxHP;
        }
    }

    /// <summary>
    /// ���� ���� �� �����ص� �÷��̾� ������ ��ġ�� ü���� ������� �÷��̾� ����
    /// </summary>
    void SetPlayerUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag(playerTag);
        for(int i = 0;i < obj.Length; i++)
        {
            obj[i].GetComponent<PlayerUnitManager>().ResetObject(playerUnits[i].pos, playerUnits[i].maxHp);
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
