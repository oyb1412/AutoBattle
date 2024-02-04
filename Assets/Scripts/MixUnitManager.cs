using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Collections;
public class MixUnitManager : MonoBehaviour
{


    /// <summary>
    /// 유닛을 구매 할 때마다 믹스 가능한 유닛을
    /// 체크해 믹스
    /// </summary>
    public void CheckUnitMix()
    {
        //유닛 보관용 리스트
        var units = new List<PlayerUnitManager>();
        var typeEndNum = new List<PlayerUnitManager>();
        var num = new List<PlayerUnitManager>();

        //활성화된 모든 유닛을 호출
        var obj = GameObject.FindGameObjectsWithTag(LevelManager.playerTag);
        if (obj.Length > 2)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                //unit리스트에 모든 유닛을 대입
                units.Add(obj[i].GetComponent<PlayerUnitManager>());
            }

            for (int j = 1; j < LevelManager.maxLevel; j++)
            {
                //레벨이 같은 유닛을 리스트로 저장
                num = units.FindAll(x => x.level == 1);
                if (num.Count > 2)
                    break;
            }
            if (num.Count > 2)
            {
                for (int i = 0; i <= (int)playerUnitType.MAGE2; i++)
                {
                    //레벨이 같은 유닛 중에서 타입이 같은 유닛을 리스트로 저장
                    typeEndNum = num.FindAll(x => x.playerUnitType == (playerUnitType)i).ToList();
                    if (typeEndNum.Count > 2)
                        break;
                }

                if (typeEndNum.Count > 2)
                {
                    //레벨,종류가 같은 유닛이 3체 이상 있을 시
                    if (typeEndNum.Count >= LevelManager.mixNum)
                    {
                        StartCoroutine(MixedUnitCorutine(typeEndNum, 1f));
                    }
                }
            }

        }
    }

    IEnumerator MixedUnitCorutine(List<PlayerUnitManager> units, float time)
    {
        units[1].transform.DOMove(units[0].transform.position, time);
        units[2].transform.DOMove(units[0].transform.position, time);

        units[1].transform.DOScale(Vector2.zero, time);
        units[2].transform.DOScale(Vector2.zero, time);

        yield return new WaitForSeconds(time);
        UnitLevelUp(units);
        DestroyUnit(units);
    }

    void DestroyUnit(List<PlayerUnitManager> unit)
    {
        for (int i = 1; i < 3; i++)
        {
            //재료로 소모된 유닛이 있던 보관함을 비움
            if (BuySelectUnit.summonIndex[unit[i].buyUnitIndex])
                BuySelectUnit.summonIndex[unit[i].buyUnitIndex] = false;

            //유닛이 보관함에 있었다면, 현재 보관함의 수용 숫자 감소
            if (unit[i].transform.position.x < GameManager.instance.playerUnitController.rimitPos[1])
                BuySelectUnit.currentActiveUnitNum--;

            //재료로 소모된 유닛 삭제
            unit[i].DeleteOtherObject();
            Destroy(unit[i].gameObject);
        }
    }
    void UnitLevelUp(List<PlayerUnitManager> unit)
    {
        unit[0].UnitLevelUp();
    }
}
