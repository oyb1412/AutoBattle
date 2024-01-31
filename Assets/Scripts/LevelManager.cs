using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� ������Ʈ ������ �迭
public enum StateType { BATTLE, NONBATTLE, WIN, LOSE}
public class LevelManager : MonoBehaviour
{
    [HideInInspector]public StateType currentState;
    public static LevelManager instance;

    //���� string���ڿ��� ����� ����
    public const string playerLayer = "PlayerUnit";
    public const string enemyLayer = "EnemyUnit";
    public const string enemyTag = "Enemy";
    public const string playerTag = "Player";
    public const string enemyMeleeAttackTag = "EnemyMeleeAttack";
    public const string playerMeleeAttackTag = "PlayerMeleeAttack";
    public const string bulletTag = "Bullet";

    [Header("NextStageGameObject")]
    [SerializeField] GameObject doNextStageBtn;

    [Header("PlayerUnitStatus")]
    //���� �� ���
    public const int buyCost = 10;
    //�Ǹ� �� ���
    public const int sellCost = 5;
    //���� �� ���
    public const int reRullCost = 2;

    //���� ����
    [HideInInspector]public int currentRound;

    //���� ���� ���
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
    /// ��Ʋ �����϶�, ������ ���� �Ǵ��ؼ� ��Ʋ ���¸� ����.
    /// ������ ���������� ����
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
    /// Start��ư Ŭ������ �ߵ�
    /// ������Ʈ�� ����
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
