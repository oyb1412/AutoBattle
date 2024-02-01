using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MixUnitManager : MonoBehaviour
{


    /// <summary>
    /// ������ ���� �� ������ �ͽ� ������ ������
    /// üũ�� �ͽ�
    /// </summary>
    public void CheckUnitMix()
    {
        //���� ������ ����Ʈ
        var units = new List<PlayerUnitManager>();
        var typeEndNum = new List<PlayerUnitManager>();
        var num = new List<PlayerUnitManager>();

        //Ȱ��ȭ�� ��� ������ ȣ��
        var obj = GameObject.FindGameObjectsWithTag(LevelManager.playerTag);

        for (int i = 0; i <obj.Length; i++)
        {
            //unit����Ʈ�� ��� ������ ����
            units.Add(obj[i].GetComponent<PlayerUnitManager>());
        }

        for (int j = 0; j < LevelManager.maxLevel; j++)
        {
            //������ ���� ������ ����Ʈ�� ����
            num = units.FindAll(x => x.playerUnitStatus.level == j + 1).ToList();
            if (num.Count > 2)
                break;
        }
        for (int i = 0; i <= (int)UnitSmallType.MAGE2; i++)
        {
            //������ ���� ���� �߿��� Ÿ���� ���� ������ ����Ʈ�� ����
            typeEndNum = num.FindAll(x => x.unitSmallType == (UnitSmallType)i).ToList();
            if (typeEndNum.Count > 2)
                break;
        }

        //����,������ ���� ������ 3ü �̻� ���� ��
        if (typeEndNum.Count >= LevelManager.mixNum)
        {
            //��� ����
            DestroyUnit(typeEndNum);
            //�ռ�
            UnitLevelUp(typeEndNum);
        }
    }

    void DestroyUnit(List<PlayerUnitManager> unit)
    {
        for (int i = 1; i < 3; i++)
        {
            //���� �Ҹ�� ������ �ִ� �������� ���
            if (BuySelectUnit.summonIndex[unit[i].buyUnitIndex])
                BuySelectUnit.summonIndex[unit[i].buyUnitIndex] = false;

            //������ �����Կ� �־��ٸ�, ���� �������� ���� ���� ����
            if (unit[i].transform.position.x < GameManager.instance.playerUnitController.rimitPos[1])
                BuySelectUnit.currentActiveUnitNum--;

            //���� �Ҹ�� ���� ����
            Destroy(unit[i].gameObject);

        }
    }
    void UnitLevelUp(List<PlayerUnitManager> unit)
    {
        //���� ������ �Լ� ȣ��
        unit[0].UnitLevelUp();
    }
}
