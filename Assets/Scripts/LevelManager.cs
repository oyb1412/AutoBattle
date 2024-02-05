using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEditor.Progress;
//현재 스테이트 구별용 배열
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE, WAIT, SELECT}
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

    //소환용 밀리 애너미 유닛 프리펩
    [SerializeField] GameObject[] enemyMeleeUnitPrefabs;  
    //소환용 레인지 애너미 유닛 프리펩
    [SerializeField] GameObject[] enemyRangeUnitPrefabs;  
    //소환용 탱크 애너미 유닛 프리펩
    [SerializeField] GameObject[] enemyTankUnitPrefabs;

    //소환용 포지션
    Vector2[,] summonPos;

    //전투 시작전 유닛 저장용
    struct PlayerUnits
    {
        public float maxHp;
        public Vector2 pos;
    }

    //전투 시작전 유닛 저장용
    [SerializeField]PlayerUnits[] playerUnits;

    [SerializeField] PlayerUnitManager[] players;

    //아이템 선택 판넬
    [SerializeField] GameObject selectPanel;

    //각각 아이템 판넬
    [SerializeField] GameObject[] itemPanel;

    void Start()
    {
        currentState = StateType.NONBATTLE;
        doNextStageBtn.SetActive(true);
        currentRound = 1;
        currentRoundText.text = currentRound + " 라운드";
        currentGoldText.text = currentGold + " 골드";
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

        yield return time;
        currentState = StateType.SELECT;
        SetSelectItem();
        currentRound++;
        currentRoundText.text = currentRound + " 라운드";
        SummonEnemyUnit(currentRound);
        centerLine.SetActive(true);
    }

    /// <summary>
    /// 선택지 선택
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
    /// 특정 아이템 생성
    /// </summary>
    public void CreateItem(int index)
    {
        //착용중인 아이템 정보를 토대로 아이템 생성
        var item = Instantiate(GameManager.instance.itemPrefabs[index], null);
        //아이템이 도달할 위치를 전장 내로 랜덤하게 계산
        var rimitpos = GameManager.instance.playerUnitController.rimitPos;
        var ranx = Random.Range(rimitpos[1], rimitpos[0]);
        var rany = Random.Range(rimitpos[2], rimitpos[3]);
        //아이템을 랜덤 위치로 1.5초에 걸쳐 이동
        item.transform.DOMove(new Vector2(ranx, rany), 1.5f);
    }

    /// <summary>
    /// 랜덤 선택지 팝업
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
    /// 전투씬으로 돌입 전 대기
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
    /// 전투 시작 전 모든 플레이어 유닛의 위치와 자기자신 프리펩을 저장
    /// </summary>
    void SavePlayerUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag(playerTag);
        //playerUnits = new PlayerUnits[obj.Length];
        //for (int i = 0; i < obj.Length; i++)
        //{
        //    //unit구조체에 기본 정보
        //    playerUnits[i].pos = obj[i].transform.position;
        //    playerUnits[i].maxHp = obj[i].GetComponent<PlayerUnitManager>().maxHP;
        //}
        players = new PlayerUnitManager[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            //unit구조체에 기본 정보
            players[i] = obj[i].GetComponent<PlayerUnitManager>();
        }
    }

    /// <summary>
    /// 전투 종료 후 저장해둔 플레이어 유닛의 위치와 체력을 기반으로 플레이어 리셋
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
