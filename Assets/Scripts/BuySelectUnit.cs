using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuySelectUnit : MonoBehaviour
{
    //최대 보관가능 유닛 수를 확인하기 위한 오브젝트
    [Header("ShowRandomUnit")]
    [SerializeField] ShowRandomUnit showRandomUnit;

    //실 생성용 프리펩 오브젝트
    [Header("UnitPrefabs")]
    [SerializeField] GameObject[] unitPrefabs;

    //보관 위치 5곳의 위치벡터
    [Header("SummonPosition")]
    [SerializeField] Vector2[] summonPos;

    //생성한 유닛들의 부모 오브젝트(더미)
    [Header("ObjectParent")]
    [SerializeField] Transform createObjectParent;

    //현재 보관되고있는 유닛의 수
    [Header("ActiveSelfUnit")]
    public static int currentActiveUnitNum;

    //보관함의 유닛의 유무 판단용 bool
    public static bool[] summonIndex;

    [SerializeField] PlayerUnitController playerUnitController;


    private void Awake()
    {
        summonIndex = new bool[5];
    }

    /// <summary>
    /// 유닛 아이콘 클릭시 작동하는 유닛 구입 함수
    /// </summary>
    public void BuySelectUnitClick(int index)
    {
        //유닛 터치상태가 아니고, 전투상태가 아니고, 현재 골드가 구입 비용보다 적으면 리턴
        if (playerUnitController.isTouch || LevelManager.instance.currentState == StateType.BATTLE &&
            LevelManager.buyCost > LevelManager.instance.currentGold)
            return;
        int saveIndex = 0;
        for(int i = 0; i < summonIndex.Length; i++)
        {
            if (summonIndex[i] == false)
            {
                saveIndex = i;
                break;
            }

        }
        //현재 보관중인 유닛 수가 최대 보관 가능한 유닛 수보다 적을때만 소환
        if (currentActiveUnitNum < showRandomUnit.showMaxUnit)
        {
            var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity, createObjectParent);
            unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
            summonIndex[saveIndex] = true;
            currentActiveUnitNum++;
            LevelManager.instance.SetGold(-LevelManager.buyCost);
        }
        else
        {
            //유닛이 꽉 찼으므로 UI등으로 경고 표시
        }

        MixUnit();
    }
    /// <summary>
    /// 전장과 보관함에 같은 종류, 같은 레벨의 유닛이 3체 이상 있을 시
    /// 유닛을 합체하는 함수(제작예정)
    /// </summary>
    void MixUnit()
    {

    }
}
