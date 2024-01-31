using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
        if (LevelManager.instance.currentState == StateType.BATTLE)
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
                //������ ���� �ۿ� ���� ���
                if (target.transform.position.x < rimitPos[1])
                {
                    //���� ���콺�� ��ġ�� ���������� ���
                    if (MouseRayCast("SellBin", "SellBin") != null)
                    {
                        isTouch = false;
                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                        //������ �ִ� �������� ����ش�
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //Ȱ��ȭ�� �������� ���� �����Ѵ�
                        BuySelectUnit.currentActiveUnitNum--;
                        //���� �ǸŰ���ŭ ��带 ��´�
                        LevelManager.instance.SetGold(LevelManager.sellCost);
                        //������ �����Ѵ�
                        Destroy(target.gameObject);
                    }
                }
                //������ ���� �� ���������� �̵���Ű�ų� �����Գ��� ����(���ۿ���)
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
}
