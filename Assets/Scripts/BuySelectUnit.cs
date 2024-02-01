using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuySelectUnit : MonoBehaviour
{
    //�ִ� �������� ���� ���� Ȯ���ϱ� ���� ������Ʈ
    [Header("ShowRandomUnit")]
    [SerializeField] ShowRandomUnit showRandomUnit;

    //�� ������ ������ ������Ʈ
    [Header("UnitPrefabs")]
    [SerializeField] GameObject[] unitPrefabs;

    //���� ��ġ 5���� ��ġ����
    [Header("SummonPosition")]
    [SerializeField]public Vector2[] summonPos;

    //������ ���ֵ��� �θ� ������Ʈ(����)
    [Header("ObjectParent")]
    [SerializeField] Transform createObjectParent;

    //���� �����ǰ��ִ� ������ ��
    [Header("ActiveSelfUnit")]
    public static int currentActiveUnitNum;

    //�������� ������ ���� �Ǵܿ� bool
    public static bool[] summonIndex;
    [SerializeField] PlayerUnitController playerUnitController;

    public bool[] save;
    public int save2;
    //���� �ͽ� üũ�� ���� ������Ʈ
    [SerializeField] MixUnitManager mixUnitManager;
    private void Awake()
    {
        summonIndex = new bool[5];
        save = new bool[5];
    }
    private void Update()
    {
        save = summonIndex;
        save2 = currentActiveUnitNum;
    }
    /// <summary>
    /// ���� ������ Ŭ���� �۵��ϴ� ���� ���� �Լ�
    /// </summary>
    public void BuySelectUnitClick(int index)
    {
        //���� ��ġ���°� �ƴϰ�, �������°� �ƴϰ�, ���� ��尡 ���� ��뺸�� ������ ����
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
        //���� �������� ���� ���� �ִ� ���� ������ ���� ������ �������� ��ȯ
        if (currentActiveUnitNum < showRandomUnit.showMaxUnit)
        {

            var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity, createObjectParent);
            unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
            summonIndex[saveIndex] = true;
            currentActiveUnitNum++;
            LevelManager.instance.SetGold(-LevelManager.buyCost);
            mixUnitManager.CheckUnitMix();

        }
        else
        {
            //������ �� á���Ƿ� UI������ ��� ǥ��
        }

    }

}
