using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
public class PlayerUnitController : MonoBehaviour
{
    //���� ��ġ���� �Ǵ�
    [Header("TouchStatus")]
    public bool isTouch;

    //��ġ�� ������ �����ϱ� ���� ������Ʈ
    [Header("TargetUnit")]
    PlayerUnitManager target;
    public float touchDelay;

    //���� ��ġ ������ ������
    [Header("RimitPos, �����")]
    public float[] rimitPos;

    //�� �߾ӿ� ǥ�õǴ� ��ġ�� ������ ���� ǥ���ϴ� �ؽ�Ʈ
    [Header("ActiveUnitNumText")]
    [SerializeField] Text activeUnitNumText;

    //���� ��ġ�� ������ ��
    [Header("CurrentActiveUnitNum")]
    [HideInInspector] public int currentActiveUnitNum;

    //��ġ ������ ������ �ִ� ��
    const int maxActiveUnitNum = 9;

    Vector2 savePosition;

    [SerializeField] Text[] synageNumText;
    [SerializeField] Text[] synageLevelText;

    public int[] activeUnitTypeNum;
    public int[] synageLevel;

    [SerializeField] SynageData[] data;
    private void Start()
    {
        activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);
        for (int i = 0; i < 3; i++)
        {
            synageLevel[i] = activeUnitTypeNum[i] / 3;
            synageLevelText[i].text = synageLevel[0] + "����";
            synageNumText[i].text = activeUnitTypeNum[0] + "/ 9";
        }
    }
    private void Update()
    {
        //���� ������ ��� ����
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;

        TouchPlayerUnit();

        if(isTouch)
            target.transform.position = GameManager.instance.mousePos;
    }

    /// <summary>
    /// ������ ��ġ�ϰų� �Ǹ�
    /// </summary>
    void TouchPlayerUnit()
    {
        //��ġ���°� �ƴϰ�, ���� ������Ʈ�� �� ��Ʋ ���°�, �������� ��ġ���� ���� �����϶�
        if(Input.GetMouseButtonDown(0) && !isTouch&& GameManager.instance.levelManager.currentState == StateType.NONBATTLE && !GameManager.instance.itemController.isTouch &&
            GameManager.instance.itemController.touchDelay > 0.1f)
        {
            //���콺 ��ġ�� �÷��̾� ������ ������ ��
            if (GameManager.instance.MouseRayCast("Player", "PlayerUnit") != null)
            {
                //target�� �÷��̾� ���� ���� ����
                target = GameManager.instance.MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();

                //target�� null�� �ƴϸ�
                if (target != null)
                {
                    //��ġ ������ ��ġ�� ����
                    savePosition = target.transform.position;

                    //�����Կ� �ִ� ������ Ŭ�� ��
                    if (target.transform.position.x < rimitPos[1])
                        savePosition = target.transform.position;
                    else
                        savePosition = new Vector2(10f, 5f);
                    isTouch = true;
                    touchDelay = 0;
                }
            }
        }

        //��ġ�����϶� ���콺 Ŭ�� ��
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
                //���忡 ��ġ ������ ����ŭ �̹� ���� ��ġ�Ǿ����� ���
                if (currentActiveUnitNum >= maxActiveUnitNum)
                {
                    isTouch = false;
                    //�����ص� ���� ��ġ�� �̵�
                    target.transform.position = savePosition;
                }
                //������ ���� ���� ���Ұ�, ���� ��ġ���� �ִ� ��ġ������ ���� ���
                if (target.transform.position.x < rimitPos[0] && target.transform.position.x > rimitPos[1] &&
                    target.transform.position.y > rimitPos[2] && target.transform.position.y < rimitPos[3] &&
                    currentActiveUnitNum < maxActiveUnitNum)
                {

                    //������ ���¸� wait�� ����
                    target.currentUnitState = unitState.WAIT;
                    //������ �ִ� �������� ����ش�
                    BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                    //Ȱ��ȭ�� �������� ���� �����Ѵ�
                    BuySelectUnit.currentActiveUnitNum--;
                    //���忡 ��ġ�� ���ڸ� �����Ѵ�
                    currentActiveUnitNum++;
                    //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                    SynageTextSet(1);
                    touchDelay = 0;
                    isTouch = false;
                }

                    //���� ���콺�� ��ġ�� ���������� ���
                    if (GameManager.instance.MouseRayCast(LevelManager.SellBinTagNLayer, LevelManager.SellBinTagNLayer) != null)
                    {
                        touchDelay = 0;
                        isTouch = false;
                        //������ �ִ� �������� ����ش�
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //Ȱ��ȭ�� �������� ���� �����Ѵ�
                        BuySelectUnit.currentActiveUnitNum--;
                        //���� �ǸŰ���ŭ ��带 ��´�
                        GameManager.instance.levelManager.SetGold(LevelManager.sellCost);
                        //������ �����Ѵ�
                        target.currentUnitState = unitState.DIE;

                        //���忡�� ������������ ������ �ű� ���
                        if (savePosition.x > rimitPos[1])
                        {
                            SynageTextSet(-1);

                        }
                }
                    //���� ���콺�� ��ġ�� �������� ���
                    if(GameManager.instance.MouseRayCast(LevelManager.SpawnColliderLayer) != null)
                    {
                        var col = GameManager.instance.MouseRayCast(LevelManager.SpawnColliderLayer);
                        //�������� null�� �ƴҶ�
                        if (col != null)
                        {
                            //������ �ִ� �������� ����ش�
                            BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                            //������ �������� �±׷� ����
                            switch (col.tag)
                            {
                                case LevelManager.SpawnColliderTag0:
                                    if(BuySelectUnit.summonIndex[0] == false)
                                    {
                                        //���忡�� ���������� ������ �ű� ���
                                        if (savePosition.x > rimitPos[1])
                                        {
                                            //�������� �� ���� ���� ���
                                            if (currentActiveUnitNum < maxActiveUnitNum)
                                            {
                                                //Ȱ��ȭ�� �������� ���� �����Ѵ�
                                                BuySelectUnit.currentActiveUnitNum++;
                                                //���忡 ��ġ�� ���ڸ� �����Ѵ�
                                                currentActiveUnitNum--;
                                                //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                                                activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);
                                            }
                                        }
                                        target.buyUnitIndex = 0;
                                        BuySelectUnit.summonIndex[0] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[0];
                                        touchDelay = 0;
                                         isTouch = false;
                                    }
                                    break;                            
                                case LevelManager.SpawnColliderTag1:
                                    if (BuySelectUnit.summonIndex[1] == false)
                                    {
                                        //���忡�� ���������� ������ �ű� ���
                                        if (savePosition.x > rimitPos[1])
                                        {
                                            //�������� �� ���� ���� ���
                                            if (currentActiveUnitNum < maxActiveUnitNum)
                                            {
                                                //Ȱ��ȭ�� �������� ���� �����Ѵ�
                                                BuySelectUnit.currentActiveUnitNum++;
                                                //���忡 ��ġ�� ���ڸ� �����Ѵ�
                                                currentActiveUnitNum--;
                                                //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                                                activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);
                                            SynageTextSet(-1);
                                        }
                                    }
                                        target.buyUnitIndex = 1;
                                        BuySelectUnit.summonIndex[1] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[1];
                                        touchDelay = 0;
                                         isTouch = false;
                                    }
                                    break;                             
                                case LevelManager.SpawnColliderTag2:
                                    if(BuySelectUnit.summonIndex[2] == false)
                                    {
                                        //���忡�� ���������� ������ �ű� ���
                                        if (savePosition.x > rimitPos[1])
                                        {
                                            //�������� �� ���� ���� ���
                                            if (currentActiveUnitNum < maxActiveUnitNum)
                                            {
                                                //Ȱ��ȭ�� �������� ���� �����Ѵ�
                                                BuySelectUnit.currentActiveUnitNum++;
                                                //���忡 ��ġ�� ���ڸ� �����Ѵ�
                                                currentActiveUnitNum--;
                                                //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                                                activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                                            //�ó��� ����
                                            SynageTextSet(-1);

                                        }
                                    }
                                        target.buyUnitIndex = 2;
                                        BuySelectUnit.summonIndex[2] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[2];
                                         touchDelay = 0;
                                          isTouch = false;
                                    }
                                    break;                              
                                case LevelManager.SpawnColliderTag3:
                                    if(BuySelectUnit.summonIndex[3] == false)
                                    {
                                        //���忡�� ���������� ������ �ű� ���
                                        if (savePosition.x > rimitPos[1])
                                        {
                                            //�������� �� ���� ���� ���
                                            if (currentActiveUnitNum < maxActiveUnitNum)
                                            {
                                                //Ȱ��ȭ�� �������� ���� �����Ѵ�
                                                BuySelectUnit.currentActiveUnitNum++;
                                                //���忡 ��ġ�� ���ڸ� �����Ѵ�
                                                currentActiveUnitNum--;
                                                //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                                                activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);
                                            SynageTextSet(-1);
                                        }
                                    }
                                        target.buyUnitIndex = 3;
                                        BuySelectUnit.summonIndex[3] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[3];
                                        touchDelay = 0;
                                          isTouch = false;
                                    }
                                    break;                            
                                case LevelManager.SpawnColliderTag4:
                                    if(BuySelectUnit.summonIndex[4] == false)
                                    {
                                        //���忡�� ���������� ������ �ű� ���
                                        if (savePosition.x > rimitPos[1])
                                        {
                                            //�������� �� ���� ���� ���
                                            if (currentActiveUnitNum < maxActiveUnitNum)
                                            {
                                                //Ȱ��ȭ�� �������� ���� �����Ѵ�
                                                BuySelectUnit.currentActiveUnitNum++;
                                                //���忡 ��ġ�� ���ڸ� �����Ѵ�
                                                currentActiveUnitNum--;
                                                //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                                                activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                                            SynageTextSet(-1);

                                            
                                            }
                                        }
                                        target.buyUnitIndex = 4;
                                        BuySelectUnit.summonIndex[4] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[4];
                                         touchDelay = 0;
                                           isTouch = false;
                                    }
                                    break;
                            }


                        }
                    }
        }
    }

    //�ó��� �ؽ�Ʈ ǥ�ÿ� ��ġ ����(������ ��ġ�� ������ ����)
    void SynageTextSet(int index)
    {
        //�ó��� ����
        switch (target.currentUnitType)
        {
            case unitType.MELEE:
                activeUnitTypeNum[0] += index;
                break;
            case unitType.RANGE:
                activeUnitTypeNum[1] += index;
                break;
            case unitType.MAGE:
                activeUnitTypeNum[2] += index;
                break;
        }
        for(int i = 0;i<3;i++)
        {
            synageLevel[i] = activeUnitTypeNum[i] / 3;
                        synageLevelText[i].text = synageLevel[i] + "����";
            synageNumText[i].text = activeUnitTypeNum[i] + "/ 9";
        }
        if (synageLevel[0] > 0 || synageLevel[1] > 0 || synageLevel[2] > 0)
        {
            var unit = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0;i<unit.Length;i++)
            {
                if (unit[i].GetComponent<PlayerUnitManager>().currentUnitType == unitType.MELEE)
                {
                    switch (synageLevel[0])
                    {
                        case 1:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                                (0.01f * data[0].upAttackDamage[1]);
                            break;
                        case 2:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                                 (0.01f * data[0].upAttackDamage[2]);
                            break;
                        case 3:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                                 (0.01f * data[0].upAttackDamage[3]);
                            break;
                    }
                }
                else if(unit[i].GetComponent<PlayerUnitManager>().currentUnitType == unitType.RANGE)
                {
                    switch (synageLevel[1])
                    {
                        case 1:
                            unit[i].GetComponent<PlayerUnitManager>().attackSpeed = unit[i].GetComponent<PlayerUnitManager>().saveAttackSpeed * 1 +
                                (0.01f * data[1].upAttackSpeed[1]);
                            unit[i].GetComponent<PlayerUnitManager>().attackRange = unit[i].GetComponent<PlayerUnitManager>().saveAttackRange * 1 +
                                (0.01f * data[1].upAttackRange[1]);
                            break;
                        case 2:
                            unit[i].GetComponent<PlayerUnitManager>().attackSpeed = unit[i].GetComponent<PlayerUnitManager>().saveAttackSpeed * 1 +
                                 (0.01f * data[1].upAttackSpeed[2]);
                            unit[i].GetComponent<PlayerUnitManager>().attackRange = unit[i].GetComponent<PlayerUnitManager>().saveAttackRange * 1 +
                                (0.01f * data[1].upAttackRange[2]);
                            break;
                        case 3:
                            unit[i].GetComponent<PlayerUnitManager>().attackSpeed = unit[i].GetComponent<PlayerUnitManager>().saveAttackSpeed * 1 +
                                  (0.01f * data[1].upAttackSpeed[3]);
                            unit[i].GetComponent<PlayerUnitManager>().attackRange = unit[i].GetComponent<PlayerUnitManager>().saveAttackRange * 1 +
                                 (0.01f * data[1].upAttackRange[3]);
                            break;
                    }
                }
                else
                {
                    switch (synageLevel[2])
                    {
                        case 1:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                             (0.01f * data[2].downResistance[1]);
                            break;
                        case 2:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                             (0.01f * data[2].downResistance[2]);
                            break;
                        case 3:
                            unit[i].GetComponent<PlayerUnitManager>().attackDamage = unit[i].GetComponent<PlayerUnitManager>().saveAttackDamage * 1 +
                             (0.01f * data[2].downResistance[3]);
                            break;
                    }
                }
            }

        }
    }





}
