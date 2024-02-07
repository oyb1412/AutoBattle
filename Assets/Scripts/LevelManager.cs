using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEditor.Progress;
//현재 스테이트 구별용 배열
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE, WAIT, SELECT,GAMEOVER}
public class LevelManager : MonoBehaviour
{
    public StateType currentState;

    [SerializeField] GotoScene scene;
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

    //아이템 선택 페이드 판넬
    [SerializeField] GameObject selectFadePanel;

    //각각 아이템 판넬
    [SerializeField] GameObject[] itemPanel;

    //플레이어 저장
    [SerializeField] GameObject[] savePlayer;   
    
    //애너미 저장
    [SerializeField] GameObject[] saveEnemy;

    //에러 메시지 판넬
    public GameObject errorMessagePanel;

    //에러 메시지 텍스트
    public Text errorMessageText;

    //플레이어 체력 표시 obj
    [SerializeField] GameObject heart;

    //플레이어 피격 표시 obj
    [SerializeField] Image hitPanel;

    void Start()
    {
        GameManager.instance.audioManager.PlayerBgm(1, true);

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
            if(target.Length <= 1)
            {
                //전투 패배
                if (target[0].GetComponent<PlayerUnitManager>() != null)
                {
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.HIT);

                    StartCoroutine(RoundLoseCorutine(3f));
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

    IEnumerator HitEffectCorutine(float size)
    {
        hitPanel.gameObject.SetActive(true);
        heart.GetComponent<RectTransform>().sizeDelta = new Vector2(size, heart.GetComponent<RectTransform>().sizeDelta.y);
        Camera.main.transform.DOShakePosition(1f, 0.5f);
        float alpha = 0.5f;
        while(true)
        {
            alpha -= Time.deltaTime * 0.5f;
            hitPanel.color = new Color(1f,0f,0f,alpha);
            yield return null;
            if(alpha <= 0)
            {
                hitPanel.gameObject.SetActive(false);
                hitPanel.color = Color.red;
                break;
            }
        }
    }

    /// <summary>
    /// 전투 패배 후 지연시간
    /// </summary>
    /// <param name="exittime"></param>
    /// <returns></returns>
    IEnumerator RoundLoseCorutine(float exittime)
    {
        switch (heart.GetComponent<RectTransform>().sizeDelta.x)
        {
            //게임 오버
            case 100:
                currentState = StateType.GAMEOVER;
                StartCoroutine(HitEffectCorutine(0));
                StopAllCoroutines();
                GameManager.instance.audioManager.PlayerBgm(1, false);

                scene.GoToScene(2);
                break;
            case 200:
                StartCoroutine(HitEffectCorutine(100f));
                break;
            case 300:
                StartCoroutine(HitEffectCorutine(200f));
                break;
        }
        if (currentState != StateType.GAMEOVER)
        {
            var time = new WaitForSeconds(exittime);
            SetAllUnit();
            currentState = StateType.LOSE;
            centerLine.transform.DOMoveY(0f, exittime);
            yield return time;
            currentState = StateType.SELECT;
            SetSelectItem();
            currentRoundText.text = currentRound + " 라운드";
            centerLine.SetActive(true);
        }
    }

    /// <summary>
    /// 선택지 선택
    /// 버튼으로 발동
    /// </summary>
    public void SelectItemClick(int index)
    {
        for (int i = 0; i < itemPanel.Length; i++)
        {
            itemPanel[i].gameObject.SetActive(false);
        }
        selectFadePanel.gameObject.SetActive(false);
        GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

        currentState = StateType.NONBATTLE;
        mainPanel.GetComponent<RectTransform>().DOAnchorPosY(-430f, 3f);
        doNextStageBtn.GetComponent<RectTransform>().DOAnchorPosY(-760f, 3f);
        if (index == 6)
        {
            SetGold(30);
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
        selectFadePanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 현재 라운드에 따라 몬스터 소환
    /// </summary>
    /// <param name="round"></param>
    void SummonEnemyUnit(int round)
    {
        if(round != 1)
            GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SUMMON);

        if (round > 5)
            round = round % 5;

        switch (round)
        {
            case 1:
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[0, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[3, 3], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[0, 4], Quaternion.identity);
                break;
            case 2:
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[0, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[0, 4], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 4], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 2], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 2], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 4], Quaternion.identity);
                break;
            case 3:
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[0, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 4], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 3], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 1], Quaternion.identity);
                break;
            case 4:
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 2], Quaternion.identity);
                Instantiate(enemyMeleeUnitPrefabs[currentRound / 5], summonPos[1, 3], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 1], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 2], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 5], summonPos[2, 3], Quaternion.identity);
                break;
            case 5:
                Instantiate(enemyTankUnitPrefabs[currentRound / 6], summonPos[0, 1], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 6], summonPos[2, 2], Quaternion.identity);
                Instantiate(enemyRangeUnitPrefabs[currentRound / 6], summonPos[2, 3], Quaternion.identity);
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
        SaveUnit();
        yield return time;
        currentState = StateType.BATTLE;
    }

    /// <summary>
    /// 전투 시작 전 모든 플레이어 유닛의 위치와 자기자신 프리펩을 저장
    /// 전투 시작 전 모든 애너미 유닛의 위치와 자기자신 프리펩을 저장
    /// </summary>
    void SaveUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag("Player");
        savePlayer = new GameObject[obj.Length];
        for(int i = 0;i<obj.Length;i++)
        {
            savePlayer[i] = Instantiate(obj[i], null);
            savePlayer[i].GetComponent<PlayerUnitManager>().SetOtherObject(false);
            savePlayer[i].SetActive(false);
        }

        obj = GameObject.FindGameObjectsWithTag("Enemy");
        saveEnemy = new GameObject[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            saveEnemy[i] = Instantiate(obj[i], null);
            saveEnemy[i].GetComponent<EnemyUnitManager>().SetOtherObject(false);
            saveEnemy[i].SetActive(false);
        }
    }

    /// <summary>
    /// 전투 승리 시 저장해둔 플레이어 유닛의 위치와 체력을 기반으로 플레이어 리셋
    /// </summary>
    void SetPlayerUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].GetComponent<PlayerUnitManager>().DeleteOtherObject();
            Destroy(obj[i]);
        }
        for (int i = 0; i< savePlayer.Length;i++)
        {
            savePlayer[i].SetActive(true);
            savePlayer[i].GetComponent<PlayerUnitManager>().SetStarObject(true);
        }
        GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SUMMON);

    }

    /// <summary>
    /// 전투 패배 시 저장해둔 모든 유닛의 위치와 체력을 기반으로 유닛 리셋
    /// </summary>
    void SetAllUnit()
    {
        var obj = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].GetComponent<EnemyUnitManager>().DeleteOtherObject();
            Destroy(obj[i]);
        }
        for (int i = 0; i < saveEnemy.Length; i++)
        {
            saveEnemy[i].SetActive(true);
        }

        SetPlayerUnit();
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
            SetErrorMessage("배치된 유닛이 존재하지 않아 전투를 시작할 수 없습니다!");

        }
        else if (currentState == StateType.NONBATTLE)
        {
            StartCoroutine(GoBattleStateCorutine(3f));
            GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.BATTLEON);

        }
    }

    public void SetGold(int value)
    {
        currentGold += value;
        currentGoldText.text = currentGold + " 골드";
    }


    /// <summary>
    /// 에러메시지 팝업 + 카메라 쉐이크
    /// </summary>
    /// <param name="text"></param>
    public void SetErrorMessage(string text)
    {
        errorMessagePanel.SetActive(true);
        errorMessageText.color = Color.white;
        errorMessageText.transform.parent.GetComponent<Image>().color = Color.white;
        errorMessageText.text = text;
        Camera.main.transform.DOShakePosition(1f,0.1f);
        errorMessageText.DOColor(new Color(1f, 1f, 1f, 0f), 1.5f);
        errorMessageText.transform.parent.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 1.5f);
        GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.ERROR);

    }
}
