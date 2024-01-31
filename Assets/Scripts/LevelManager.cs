using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StateType { BATTLE, NONBATTLE, WIN, LOSE}
public class LevelManager : MonoBehaviour
{
    [HideInInspector]public StateType currentState;
    public static LevelManager instance;
    public const string playerLayer = "PlayerUnit";
    public const string enemyLayer = "EnemyUnit";
    public const string enemyMeleeAttackTag = "EnemyMeleeAttack";
    public const string playerMeleeAttackTag = "PlayerMeleeAttack";

    [Header("NextStageGameObject")]
    [SerializeField] GameObject doNextStageBtn;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentState = StateType.NONBATTLE;
        doNextStageBtn.SetActive(true);
    }


    /// <summary>
    /// ��Ʋ �����϶�, ������ ���� �Ǵ��ؼ� ��Ʋ ���¸� ����.
    /// ������ ���������� ����
    /// </summary>
    public void ExitBattleState(string layer)
    {
        if(currentState == StateType.BATTLE) 
        {
            var target = GameObject.FindGameObjectsWithTag(layer);
            if(target.Length == 0)
            {
                currentState = StateType.NONBATTLE;
                doNextStageBtn.SetActive(true);
            }
        }
    }

    public void NextStageBtnClick()
    {
        if (currentState == StateType.NONBATTLE)
        {
             currentState = StateType.BATTLE;
             doNextStageBtn.SetActive(false);
        }
    }
}
