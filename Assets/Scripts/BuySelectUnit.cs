using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuySelectUnit : MonoBehaviour
{
    [Header("ShowRandomUnit")]
    [SerializeField] ShowRandomUnit showRandomUnit;

    [Header("UnitPrefabs")]
    [SerializeField] GameObject[] unitPrefabs;

    [Header("SummonPosition")]
    [SerializeField] Vector2[] summonPos;

    [Header("ObjectParent")]
    [SerializeField] Transform createObjectParent;

    [Header("ActiveSelfUnit")]
    public static int currentActiveUnitNum;

    public static bool[] summonIndex;

    [SerializeField] PlayerUnitController playerUnitController;


    private void Awake()
    {
        summonIndex = new bool[5];
    }
    /// <summary>
    /// ���� ������ Ŭ���� �۵��ϴ� ���� ���� �Լ�
    /// </summary>
    public void BuySelectUnitClick(int index)
    {
        if (playerUnitController.isTouch || LevelManager.instance.currentState == StateType.BATTLE)
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
        if (currentActiveUnitNum < showRandomUnit.showMaxUnit)
        {
            var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity, createObjectParent);
            unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
            summonIndex[saveIndex] = true;
            currentActiveUnitNum++;
        }
        else
        {
            //������ �� á���Ƿ� UI������ ��� ǥ��
        }
    }
}
