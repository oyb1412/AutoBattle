using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MixUnitManager : MonoBehaviour
{
    [SerializeField] BuySelectUnit buySelectUnit;

    [SerializeField] PlayerUnitController playerUnitController;
    /// <summary>
    /// 유닛을 구매 할 때마다 믹스 가능한 유닛을
    /// 체크해 믹스
    /// </summary>
    public void CheckUnitMix()
    {
        //유닛 보관용 리스트
        var units = new List<PlayerUnitManager>();

        //활성화된 모든 유닛을 호출
        var obj = GameObject.FindGameObjectsWithTag(LevelManager.playerTag);

        for (int i = 0; i <obj.Length; i++)
        {
            //unit리스트에 모든 유닛을 대입
            units.Add(obj[i].GetComponent<PlayerUnitManager>());
        }

        for (int j = 0; j < LevelManager.maxLevel; j++)
        {
            //레벨이 같은 유닛을 리스트로 저장
            var num = units.FindAll(x => x.playerUnitStatus.level == j).ToList();
            var typeEndNum = new List<PlayerUnitManager>();
            for (int i = 0; i < (int)UnitSmallType.MAGE2; i++ )
            {
                //레벨이 같은 유닛 중에서 타입이 같은 유닛을 리스트로 저장
                typeEndNum = num.FindAll(x => x.unitSmallType == (UnitSmallType)j).ToList();
            }


            Debug.Log(num.Count + "레벨이 같은 유닛 수");
            Debug.Log(typeEndNum.Count + "레벨이 같고 타입도 같은 유닛 수");

            //레벨,종류가 같은 유닛이 3체 이상 있을 시
            if (typeEndNum.Count >= LevelManager.mixNum)
            {
                //재료 제거
                DestroyUnit(typeEndNum);
                //합성
                UnitLevelUp(typeEndNum);
            }
        }
    }

    void DestroyUnit(List<PlayerUnitManager> unit)
    {
        for (int i = 1; i < 3; i++)
        {
            //재료로 소모된 유닛이 있던 보관함을 비움
            if (BuySelectUnit.summonIndex[unit[i].buyUnitIndex])
                BuySelectUnit.summonIndex[unit[i].buyUnitIndex] = false;

            //유닛이 보관함에 있었다면, 현재 보관함의 수용 숫자 감소
            if (unit[i].transform.position.x < playerUnitController.rimitPos[1])
                BuySelectUnit.currentActiveUnitNum--;

            //재료로 소모된 유닛 삭제
            Destroy(unit[i].gameObject);

        }
    }
    void UnitLevelUp(List<PlayerUnitManager> unit)
    {
        //유닛 레벨업 함수 호출
        unit[0].UnitLevelUp();
    }
}
