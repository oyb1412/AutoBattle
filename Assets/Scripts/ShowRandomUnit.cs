using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRandomUnit : MonoBehaviour
{
    //랜덤으로 활성화시킬 9개의 이미지
    [Header("UnitImages")]
    [SerializeField] GameObject[] unitImages;

    //한번에 최대 활성화 유닛 이미지 수
    [HideInInspector]public int showMaxUnit = 5;


    private void Start()
    {
        ShowRandomUnitImage();
    }

    /// <summary>
    /// 활성화되어있는 모든 유닛 이미지를 비활성화 후
    /// 랜덤한 5명의 유닛 이미지를 활성화
    /// 버튼으로 활성화
    /// </summary>
    public void RerollUnitImage()
    {
        //현재 터치중이거나, 전투 상태거나, 현재 골드가 리룰 비용보다 적으면 리턴
        if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        if(GameManager.instance.levelManager.currentGold < LevelManager.reRullCost)
        {
            GameManager.instance.levelManager.SetErrorMessage("보유 골드가 부족해 리롤을 할 수 없습니다!");
            return;
        }

        int[] random = new int[showMaxUnit];
        for (int i = 0; i < unitImages.Length; i++)
        {
            if (unitImages[i].activeSelf)
            unitImages[i].SetActive(false);
        }
        while(true)
        {
            random[0] = Random.Range(0, unitImages.Length);
            random[1] = Random.Range(0, unitImages.Length);
            random[2] = Random.Range(0, unitImages.Length);
            random[3] = Random.Range(0, unitImages.Length);
            random[4] = Random.Range(0, unitImages.Length);

            if (random[0] != random[1] && random[0] != random[2] && random[0] != random[3] && random[0] != random[4] &&
                random[1] != random[2] && random[1] != random[3] && random[1] != random[4] &&
                random[2] != random[3] && random[2] != random[4] &&
                random[3] != random[4])
                break;
        }
        for (int i = 0; i < random.Length; i++)
        {
            unitImages[random[i]].SetActive(true);
        }
        GameManager.instance.levelManager.SetGold(-LevelManager.reRullCost);
        GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.GOLD);

    }

    /// <summary>
    /// 리롤이 아닌 유닛구매했을때
    /// </summary>
    public void ShowRandomUnitImage()
    {
        //현재 터치중이거나, 전투 상태면 리턴
        if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        int[] random = new int[showMaxUnit];
        for (int i = 0; i < unitImages.Length; i++)
        {
            if (unitImages[i].gameObject.activeSelf)
                unitImages[i].SetActive(false);
        }
        while (true)
        {
            random[0] = Random.Range(0, unitImages.Length);
            random[1] = Random.Range(0, unitImages.Length);
            random[2] = Random.Range(0, unitImages.Length);
            random[3] = Random.Range(0, unitImages.Length);
            random[4] = Random.Range(0, unitImages.Length);

            if (random[0] != random[1] && random[0] != random[2] && random[0] != random[3] && random[0] != random[4] &&
                random[1] != random[2] && random[1] != random[3] && random[1] != random[4] &&
                random[2] != random[3] && random[2] != random[4] &&
                random[3] != random[4])
                break;
        }
        for (int i = 0; i < random.Length; i++)
        {
            unitImages[random[i]].SetActive(true);
        }
    }

}
