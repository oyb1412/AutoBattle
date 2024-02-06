using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRandomUnit : MonoBehaviour
{
    //�������� Ȱ��ȭ��ų 9���� �̹���
    [Header("UnitImages")]
    [SerializeField] GameObject[] unitImages;

    //�ѹ��� �ִ� Ȱ��ȭ ���� �̹��� ��
    [HideInInspector]public int showMaxUnit = 5;


    private void Start()
    {
        ShowRandomUnitImage();
    }

    /// <summary>
    /// Ȱ��ȭ�Ǿ��ִ� ��� ���� �̹����� ��Ȱ��ȭ ��
    /// ������ 5���� ���� �̹����� Ȱ��ȭ
    /// ��ư���� Ȱ��ȭ
    /// </summary>
    public void RerollUnitImage()
    {
        //���� ��ġ���̰ų�, ���� ���°ų�, ���� ��尡 ���� ��뺸�� ������ ����
        if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        if(GameManager.instance.levelManager.currentGold < LevelManager.reRullCost)
        {
            GameManager.instance.levelManager.SetErrorMessage("���� ��尡 ������ ������ �� �� �����ϴ�!");
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
    /// ������ �ƴ� ���ֱ���������
    /// </summary>
    public void ShowRandomUnitImage()
    {
        //���� ��ġ���̰ų�, ���� ���¸� ����
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
