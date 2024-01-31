using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRandomUnit : MonoBehaviour
{
    [Header("UnitImages")]
    [SerializeField] Image[] unitImages;

    [HideInInspector]public int showMaxUnit = 5;

    [SerializeField] PlayerUnitController playerUnitController;
    private void Start()
    {
        ShowRandomUnitImage();
    }

    /// <summary>
    /// Ȱ��ȭ�Ǿ��ִ� ��� ���� �̹����� ��Ȱ��ȭ ��
    /// ������ 5���� ���� �̹����� Ȱ��ȭ
    /// </summary>
    public void ShowRandomUnitImage()
    {
        if (playerUnitController.isTouch || LevelManager.instance.currentState == StateType.BATTLE)
            return;
        int[] random = new int[showMaxUnit];
        for (int i = 0; i < unitImages.Length; i++)
        {
            if (unitImages[i].gameObject.activeSelf)
            unitImages[i].gameObject.SetActive(false);
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
            unitImages[random[i]].gameObject.SetActive(true);
        }
    }

}
