using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
//현재 스테이트 구별용 배열
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE, WAIT}
public class LevelManager : MonoBehaviour
{
    public StateType currentState;

    //고정 string문자열은 상수로 저장
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
    //구입 시 비용
    public const int buyCost = 10;
    //판매 시 비용
    public const int sellCost = 5;
    //리롤 시 비용
    public const int reRullCost = 2;
    //유닛 최대 레벨
    public const int maxLevel = 3;
    //유닛 합성 조건 수
    public const int mixNum = 3;
    //현재 라운드
    [HideInInspector]public int currentRound;

    //현재 보유 골드
    public int currentGold;


    [Header("UiObject")]
    [SerializeField] Text currentGoldText;
    [SerializeField] Text currentRoundText;


    //전투 시작전 유닛 저장용
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
        currentRoundText.text = currentRound + " 라운드";
        currentGoldText.text = currentGold + " 골드";

    }


    /// <summary>
    /// 배틀 상태일때, 유닛의 수를 판단해서 배틀 상태를 종료.
    /// 유닛이 죽을때마다 실행
    /// </summary>
    public void ExitBattleState(string tag)
    {
        if(currentState == StateType.BATTLE) 
        {
            var target = GameObject.FindGameObjectsWithTag(tag);
            if(target.Length == 1)
            {
                //전투 패배
                if (target[0].GetComponent<PlayerUnitManager>() != null)
                {
                }
                //전투 승리
                else if (target[0].GetComponent<EnemyUnitManager>() != null)
                {
                    StartCoroutine(RoundWinCorutine(3f));
                }

            }
        }
    }

    /// <summary>
    /// 전투 승리 후 지연시간
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
        currentRoundText.text = currentRound + " 라운드";
        centerLine.SetActive(true);
    }

    /// <summary>
    /// 전투씬으로 돌입 전 대기
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
    /// 전투 시작 전 모든 플레이어 유닛의 위치와 자기자신 프리펩을 저장
    /// </summary>
    void SavePlayerUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag(playerTag);
        playerUnits = new PlayerUnits[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            //unit구조체에 기본 정보
            playerUnits[i].pos = obj[i].transform.position;
            playerUnits[i].maxHp = obj[i].GetComponent<PlayerUnitManager>().maxHP;
        }
    }

    /// <summary>
    /// 전투 종료 후 저장해둔 플레이어 유닛의 위치와 체력을 기반으로 플레이어 리셋
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
    /// Start버튼 클릭으로 발동
    /// 스테이트를 변경
    /// </summary>
    public void NextStageBtnClick()
    {
        if(GameManager.instance.playerUnitController.currentActiveUnitNum == 0)
        {
            //배치된 유닛이 없으면 전투 시작 불가 경고
        }
        else if (currentState == StateType.NONBATTLE)
        {
            StartCoroutine(GoBattleStateCorutine(3f));
        }
    }

    public void SetGold(int value)
    {
        currentGold += value;
        currentGoldText.text = currentGold + " 골드";
    }
}
