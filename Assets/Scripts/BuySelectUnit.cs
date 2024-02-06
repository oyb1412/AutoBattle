using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuySelectUnit : MonoBehaviour
{


    //실 생성용 프리펩 오브젝트
    [Header("UnitPrefabs")]
    [SerializeField] GameObject[] unitPrefabs;

    //보관 위치 5곳의 위치벡터
    [Header("SummonPosition")]
    [SerializeField]public Vector2[] summonPos;

    //현재 보관되고있는 유닛의 수
    [Header("ActiveSelfUnit")]
    public static int currentActiveUnitNum;

    //보관함의 유닛의 유무 판단용 bool
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
    /// 유닛 아이콘 클릭시 작동하는 유닛 구입 함수
    /// </summary>
    public void BuySelectUnitClick(int index)
    {
        //유닛 터치상태가 아니고, 전투상태가 아니고, 현재 골드가 구입 비용보다 적으면 리턴
        if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        if (LevelManager.buyCost > GameManager.instance.levelManager.currentGold)
        {
            GameManager.instance.levelManager.SetErrorMessage("보유 골드가 부족해 유닛을 구매할 수 없습니다!");
            return;
        }

        int saveIndex = 0;
        for(int i = 0; i < summonIndex.Length; i++)
        {
            if (summonIndex[i] == false)
            {
                saveIndex = i;
                break;
            }

        }
        //현재 보관중인 유닛 수가 최대 보관 가능한 유닛 수보다 적을때만 소환
        if (currentActiveUnitNum < GameManager.instance.showRandomUnit.showMaxUnit)
        {
            var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity);
            unit.GetComponent<PlayerUnitManager>().savePos = summonPos[saveIndex];
            unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
            unit.GetComponent<PlayerUnitManager>().currentUnitState = unitState.WAIT;
            summonIndex[saveIndex] = true;
            currentActiveUnitNum++;
            GameManager.instance.levelManager.SetGold(-LevelManager.buyCost);
            GameManager.instance.showRandomUnit.ShowRandomUnitImage();
            GameManager.instance.mixUnitManager.CheckUnitMix();
            GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.GOLD);

        }
        else
        {
            //유닛이 꽉 찼으므로 UI등으로 경고 표시
            GameManager.instance.levelManager.SetErrorMessage("유닛을 더이상 보관할 수 없습니다!");
        }

    }

}
