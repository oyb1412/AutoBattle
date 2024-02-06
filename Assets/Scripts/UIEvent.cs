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
                    info.text = "골드를 소모해 유닛 목록을 새로고침합니다.";
                    break;
                case "Unit0BG":
                    synagePanel.SetActive(true);
                    info.text = "균일한 밸런스를 지닌 근접 유닛입니다. 레벨 상승 시 일정 확률로 적을 기절시킵니다.";
                    break;             
                case "Unit1BG":
                    synagePanel.SetActive(true);
                    info.text = "낮은 방어력과 체력을 대가로 강한 공격력을 얻은 유닛입니다. 레벨 상승 시 여러명의 적을 공격합니다.";
                    break;             
                case "Unit2BG":
                    synagePanel.SetActive(true);
                    info.text = "빠른 속도로 적을 기습하는 암살자입니다. 레벨 상승 시 일정 확률로 치명타 피해를 입힙니다.";
                    break;             
                case "Unit3BG":
                    synagePanel.SetActive(true);
                    info.text = "균일한 밸런스를 지닌 원거리 유닛입니다. 레벨 상승 시 일정 확률로 화살을 한발 더 발사합니다.";
                    break;            
                case "Unit4BG":
                    synagePanel.SetActive(true);
                    info.text = "체력이 낮지만 빠른 속도를 지닌 궁수입니다. 레벨 상승 시 일정 확률로 화살이 적을 관통합니다.";
                    break;             
                case "Unit5BG":
                    synagePanel.SetActive(true);
                    info.text = "느리지만 긴 사거리와 강력한 공격력을 지닌 저격수입니다. 레벨 상승 시 일정 확률로 적을 즉사시킵니다.";
                    break;            
                case "Unit6BG":
                    synagePanel.SetActive(true);
                    info.text = "아군의 공격력을 강화시키는 버퍼입니다. 레벨 상승 시 아군의 공격 속도도 증가시킵니다.";
                    break;            
                case "Unit7BG":
                    synagePanel.SetActive(true);
                    info.text = "아군의 체력을 회복시키는 힐러입니다. 레벨 상승 시 회복량이 증가합니다.";
                    break;            
                case "Unit8BG":
                    synagePanel.SetActive(true);
                    info.text = "적을 불태우는 화염마법사입니다. 레벨 상승 시 일정 확률로 여러발의 화염구를 발사합니다.";
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
