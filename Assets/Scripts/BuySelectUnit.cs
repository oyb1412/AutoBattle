using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuySelectUnit : MonoBehaviour
{


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


    public bool[] save;
    public int save2;

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
        if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE ||
            LevelManager.buyCost > GameManager.instance.levelManager.currentGold)
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
        if (currentActiveUnitNum < GameManager.instance.showRandomUnit.showMaxUnit)
        {
            var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity, createObjectParent);
            unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
            unit.GetComponent<PlayerUnitManager>().currentUnitState = unitState.WAIT;
            summonIndex[saveIndex] = true;
            currentActiveUnitNum++;
            GameManager.instance.levelManager.SetGold(-LevelManager.buyCost);
            GameManager.instance.showRandomUnit.ShowRandomUnitImage();
            GameManager.instance.mixUnitManager.CheckUnitMix();

        }
        else
        {
            //������ �� á���Ƿ� UI������ ��� ǥ��
        }

    }

}
