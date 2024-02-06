using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEvent : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler

{
    GameObject data;
    [SerializeField] GameObject synagePanel;
    public SynageData[] synageData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData != null)
        {
            data = eventData.pointerCurrentRaycast.gameObject;
            var synageLevel = GameManager.instance.playerUnitController.synageLevel;
            var info = synagePanel.GetComponentInChildren<Text>();
            switch (data.name)
            {
                case "MeleeSynage":
                    synagePanel.SetActive(true);
                    info.text = string.Format(synageData[(int)unitType.MELEE].synageInfo, synageData[(int)unitType.MELEE].upAttackDamage[synageLevel[(int)unitType.MELEE]], synageData[(int)unitType.MELEE].upReflection[synageLevel[(int)unitType.MELEE]]);
                    break;     
                case "RangeSynage":
                    synagePanel.SetActive(true);
                    info.text = string.Format(synageData[(int)unitType.RANGE].synageInfo, synageData[(int)unitType.RANGE].upAttackSpeed[synageLevel[(int)unitType.RANGE]], synageData[(int)unitType.RANGE].upAttackRange[synageLevel[(int)unitType.RANGE]]);
                    break;   
                case "MageSynage":
                    synagePanel.SetActive(true);
                    info.text = string.Format(synageData[(int)unitType.MAGE].synageInfo, synageData[(int)unitType.MAGE].downResistance[synageLevel[(int)unitType.MAGE]]);
                    break;
                case "Reroll":
                    synagePanel.SetActive(true);
                    info.text = "��带 �Ҹ��� ���� ����� ���ΰ�ħ�մϴ�.";
                    break;
                case "Unit0BG":
                    synagePanel.SetActive(true);
                    info.text = "������ �뷱���� ���� ���� �����Դϴ�. ���� ��� �� ���� Ȯ���� ���� ������ŵ�ϴ�.";
                    break;             
                case "Unit1BG":
                    synagePanel.SetActive(true);
                    info.text = "���� ���°� ü���� �밡�� ���� ���ݷ��� ���� �����Դϴ�. ���� ��� �� �������� ���� �����մϴ�.";
                    break;             
                case "Unit2BG":
                    synagePanel.SetActive(true);
                    info.text = "���� �ӵ��� ���� ����ϴ� �ϻ����Դϴ�. ���� ��� �� ���� Ȯ���� ġ��Ÿ ���ظ� �����ϴ�.";
                    break;             
                case "Unit3BG":
                    synagePanel.SetActive(true);
                    info.text = "������ �뷱���� ���� ���Ÿ� �����Դϴ�. ���� ��� �� ���� Ȯ���� ȭ���� �ѹ� �� �߻��մϴ�.";
                    break;            
                case "Unit4BG":
                    synagePanel.SetActive(true);
                    info.text = "ü���� ������ ���� �ӵ��� ���� �ü��Դϴ�. ���� ��� �� ���� Ȯ���� ȭ���� ���� �����մϴ�.";
                    break;             
                case "Unit5BG":
                    synagePanel.SetActive(true);
                    info.text = "�������� �� ��Ÿ��� ������ ���ݷ��� ���� ���ݼ��Դϴ�. ���� ��� �� ���� Ȯ���� ���� ����ŵ�ϴ�.";
                    break;            
                case "Unit6BG":
                    synagePanel.SetActive(true);
                    info.text = "�Ʊ��� ���ݷ��� ��ȭ��Ű�� �����Դϴ�. ���� ��� �� �Ʊ��� ���� �ӵ��� ������ŵ�ϴ�.";
                    break;            
                case "Unit7BG":
                    synagePanel.SetActive(true);
                    info.text = "�Ʊ��� ü���� ȸ����Ű�� �����Դϴ�. ���� ��� �� ȸ������ �����մϴ�.";
                    break;            
                case "Unit8BG":
                    synagePanel.SetActive(true);
                    info.text = "���� ���¿�� ȭ���������Դϴ�. ���� ��� �� ���� Ȯ���� �������� ȭ������ �߻��մϴ�.";
                    break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData != null && data != null)
        {
            synagePanel.SetActive(false);
        }
    }

}
