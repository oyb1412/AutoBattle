using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Collections;
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
        if (obj.Length > 2)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                //unit����Ʈ�� ��� ������ ����
                units.Add(obj[i].GetComponent<PlayerUnitManager>());
            }

            for (int j = 1; j < LevelManager.maxLevel; j++)
            {
                //������ ���� ������ ����Ʈ�� ����
                num = units.FindAll(x => x.level == j);
                if (num.Count > 2)
                    break;
            }
            if (num.Count > 2)
            {
                for (int i = 0; i <= (int)playerUnitType.MAGE2; i++)
                {
                    //������ ���� ���� �߿��� Ÿ���� ���� ������ ����Ʈ�� ����
                    typeEndNum = num.FindAll(x => x.playerUnitType == (playerUnitType)i);
                    if (typeEndNum.Count > 2)
                        break;
                }

                if (typeEndNum.Count > 2)
                {
                    //����,������ ���� ������ 3ü �̻� ���� ��
                    if (typeEndNum.Count >= LevelManager.mixNum)
                    {
                        StartCoroutine(MixedUnitCorutine(typeEndNum, 0.2f));
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
        yield return new WaitForSeconds(time * 1.1f);
        CheckUnitMix();
    }

    void DestroyUnit(List<PlayerUnitManager> unit)
    {
        for (int i = 1; i < 3; i++)
        {
            //���� �Ҹ�� ������ �ִ� �������� ���
            if (BuySelectUnit.summonIndex[unit[i].buyUnitIndex])
                BuySelectUnit.summonIndex[unit[i].buyUnitIndex] = false;

            //������ �����Կ� �־��ٸ�, ���� �������� ���� ���� ����
            if (unit[i].savePos.x < GameManager.instance.playerUnitController.rimitPos[1])
                BuySelectUnit.currentActiveUnitNum--;
            //������ ���忡 �־��ٸ�, ���� ��ġ �� ���� �� �ó��� ����
            else
            {
                GameManager.instance.playerUnitController.SynageSet(-1, unit[i]);
                GameManager.instance.playerUnitController.currentActiveUnitNum--;
                GameManager.instance.playerUnitController.activeUnitNumText.text = string.Format("{0} / {1}", GameManager.instance.playerUnitController.currentActiveUnitNum, PlayerUnitController.maxActiveUnitNum);

            }


            //������ ���
            unit[i].ClearItem();
            //���� �Ҹ�� ���� ����
            unit[i].DeleteOtherObject();
            Destroy(unit[i].gameObject);
        }
    }
    void UnitLevelUp(List<PlayerUnitManager> unit)
    {
        //������ ���
        unit[0].ClearItem();
        unit[0].UnitLevelUp();
        unit[0].summonEffect.Play();
        GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.LEVELUP);

    }
}
