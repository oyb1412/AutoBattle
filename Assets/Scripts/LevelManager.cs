using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//현재 스테이트 구별용 배열
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE}
public class LevelManager : MonoBehaviour
{
    [HideInInspector]public StateType currentState;
    public static LevelManager instance;

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

    [Header("PlayerUnitStatus")]
    //구입 시 비용
    public const int buyCost = 10;
    //판매 시 비용
    public const int sellCost = 5;
    //리롤 시 비용
    public const int reRullCost = 2;
    //유닛 최대 레벨
    public const int maxLevel = 5;
    //유닛 합성 조건 수
    public const int mixNum = 3;
    //현재 라운드
    [HideInInspector]public int currentRound;

    //현재 보유 골드
    [HideInInspector]public int currentGold; 
    

    [Header("UiObject")]
    [SerializeField] Text currentGoldText;
    [SerializeField] Text currentRoundText;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        currentState = StateType.NONBATTLE;
        doNextStageBtn.SetActive(true);
        currentRound = 1;
        currentGold = 50;
        currentRoundText.text = "Round " + currentRound; 
        currentGoldText.text = "Gold " + currentGold; 
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
            Debug.Log(target.Length);
            if(target.Length == 1)
            {
                currentState = StateType.NONBATTLE;
                currentRound++;
                currentRoundText.text = "Round " + currentRound;
                doNextStageBtn.SetActive(true);
            }
        }
    }


    /// <summary>
    /// Start버튼 클릭으로 발동
    /// 스테이트를 변경
    /// </summary>
    public void NextStageBtnClick()
    {
        if (currentState == StateType.NONBATTLE)
        {
             currentState = StateType.BATTLE;
             doNextStageBtn.SetActive(false);
        }
    }

    public void SetGold(int value)
    {
        currentGold += value;
        currentGoldText.text = "Gold " + currentGold;
    }
}
