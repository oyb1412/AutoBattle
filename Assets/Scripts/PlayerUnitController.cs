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
    Vector2 mousePos;

    //��ġ�� ������ �����ϱ� ���� ������Ʈ
    [Header("TargetUnit")]
    PlayerUnitManager target;
    float touchDelay;

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


    private void Start()
    {
        activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

    }
    private void Update()
    {
        //���� ������ ��� ����
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TouchPlayerUnit();

        if(isTouch)
            target.transform.position = mousePos;
    }

    /// <summary>
    /// ������ ��ġ�ϰų� �Ǹ�
    /// </summary>
    void TouchPlayerUnit()
    {
        //��ġ���°� �ƴҶ� ���콺 Ŭ�� ��
        if(Input.GetMouseButtonDown(0) && !isTouch)
        {
            //���콺 ��ġ�� �÷��̾� ������ ������ ��
            if (MouseRayCast("Player", "PlayerUnit") != null)
            {
                //target�� �÷��̾� ���� ���� ����
                target = MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();

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
                    target.updownStatus = PlayerUnitManager.UpDownStatus.UP;
                }
            }
        }

        //��ġ�����϶� ���콺 Ŭ�� ��
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
            //������ Up�����Ͻ�
            if (target.updownStatus == PlayerUnitManager.UpDownStatus.UP)
            {
                //���忡 ��ġ ������ ����ŭ �̹� ���� ��ġ�Ǿ����� ���
                if (currentActiveUnitNum >= maxActiveUnitNum)
                {
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    isTouch = false;
                    //�����ص� ���� ��ġ�� �̵�
                    target.transform.position = savePosition;
                }
                //������ ���� ���� ���Ұ�, ���� ��ġ���� �ִ� ��ġ������ ���� ���
                if (target.transform.position.x < rimitPos[0] && target.transform.position.x > rimitPos[1] &&
                    target.transform.position.y > rimitPos[2] && target.transform.position.y < rimitPos[3] &&
                    currentActiveUnitNum < maxActiveUnitNum)
                {
                    //���忡 ��ġ
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    //������ ���¸� wait���� idle�� ����
                    target.playerUnitStatus.currentUnitState = unitState.IDLE;
                    //������ �ִ� �������� ����ش�
                    BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                    //Ȱ��ȭ�� �������� ���� �����Ѵ�
                    BuySelectUnit.currentActiveUnitNum--;
                    //���忡 ��ġ�� ���ڸ� �����Ѵ�
                    currentActiveUnitNum++;
                    //���ڸ� ���������� �ؽ�Ʈ�� �������ش�
                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                    isTouch = false;
                }

                    //���� ���콺�� ��ġ�� ���������� ���
                    if (MouseRayCast(LevelManager.SellBinTagNLayer, LevelManager.SellBinTagNLayer) != null)
                    {
                        isTouch = false;
                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                        //������ �ִ� �������� ����ش�
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //Ȱ��ȭ�� �������� ���� �����Ѵ�
                        BuySelectUnit.currentActiveUnitNum--;
                        //���� �ǸŰ���ŭ ��带 ��´�
                        GameManager.instance.levelManager.SetGold(LevelManager.sellCost);
                        //������ �����Ѵ�
                        target.playerUnitStatus.currentUnitState = unitState.DIE;
                    }
                    //���� ���콺�� ��ġ�� �������� ���
                    if(MouseRayCast(LevelManager.SpawnColliderLayer) != null)
                    {
                        var col = MouseRayCast(LevelManager.SpawnColliderLayer);
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
                                        isTouch = false;
                                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;


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
                                            }
                                        }
                                        target.buyUnitIndex = 1;
                                        BuySelectUnit.summonIndex[1] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[1];
                                        isTouch = false;
                                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
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
                                            }
                                        }
                                        target.buyUnitIndex = 2;
                                        BuySelectUnit.summonIndex[2] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[2];
                                        isTouch = false;
                                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
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
                                            }
                                        }
                                        target.buyUnitIndex = 3;
                                        BuySelectUnit.summonIndex[3] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[3];
                                        isTouch = false;
                                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
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
                                            }
                                        }
                                        target.buyUnitIndex = 4;
                                        BuySelectUnit.summonIndex[4] = true;
                                        target.transform.position = GameManager.instance.buySelectUnit.summonPos[4];
                                        isTouch = false;
                                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                                    }
                                    break;
                            }


                        }
                    }
            }
        }
    }

    public Collider2D MouseRayCast(string tag, string layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask(layer));

        if (hit.collider != null && hit.collider.CompareTag(tag))
        {
            return hit.collider;
        }
        else
            return null;
    }

    public Collider2D MouseRayCast(string layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask(layer));

        if (hit.collider != null)
        {
            return hit.collider;
        }
        else
            return null;
    }



}
