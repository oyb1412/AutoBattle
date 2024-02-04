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
            var synage = GameManager.instance.playerUnitController.synageLevel;
            switch (data.name)
            {
                case "MeleeSynageBG":
                    synagePanel.SetActive(true);
                    synagePanel.transform.position = Input.mousePosition;
                    synagePanel.GetComponentInChildren<Text>().text = string.Format(synageData[0].synageInfo, synageData[0].upAttackDamage[synage[0]], synageData[0].upReflection[synage[0]]);
                    break;     
                case "RangeSynageBG":
                    synagePanel.SetActive(true);
                    synagePanel.transform.position = Input.mousePosition;
                    synagePanel.GetComponentInChildren<Text>().text = string.Format(synageData[1].synageInfo, synageData[1].upAttackSpeed[synage[1]], synageData[1].upAttackRange[synage[1]]);
                    break;   
                case "MageSynageBG":
                    synagePanel.SetActive(true);
                    synagePanel.transform.position = Input.mousePosition;
                    synagePanel.GetComponentInChildren<Text>().text = string.Format(synageData[2].synageInfo, synageData[2].downResistance[synage[2]]);
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
