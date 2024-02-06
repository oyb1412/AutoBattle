using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ItemController : MonoBehaviour
{
    //������ ��ġ ����
    public bool isTouch;

    //�������� �ִ� ��ġ
    Vector2 savePosition;

    //��ġ�� ������
    [SerializeField] ItemManager target;

    //������ġ ������ ���� ������
    public float touchDelay;


    private void Update()
    {
        //���� ������ ��� ����
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;

        TouchItem();

        if (isTouch)
            target.transform.position = GameManager.instance.mousePos;
    }
    /// <summary>
    /// ������ ����
    /// </summary>
    void TouchItem()
    {
        //��ġ���°� �ƴϰ�, ���� ������Ʈ�� �� ��Ʋ ���°�, �÷��̾ ��ġ���� ���� �����϶�
        if (Input.GetMouseButtonDown(0) && !isTouch && GameManager.instance.levelManager.currentState == StateType.NONBATTLE && !GameManager.instance.playerUnitController.isTouch &&
            GameManager.instance.playerUnitController.touchDelay > 0.1f)
        {
            //���콺 ��ġ�� �������� ������ ��
            if (GameManager.instance.MouseRayCast("Item") != null)
            {
                //target�� ������ ���� ����
                target = GameManager.instance.MouseRayCast("Item").GetComponent<ItemManager>();

                //target�� null�� �ƴϸ�
                if (target != null)
                {
                    //��ġ ������ ��ġ�� ����
                    savePosition = target.transform.position;
                    isTouch = true;
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

                    touchDelay = 0;
                }
            }
        }

        //��ġ�����϶� ���콺 Ŭ�� ��
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
            //�������� ���� ���� ������ ���
            if (target.transform.position.x < GameManager.instance.playerUnitController.rimitPos[0] && target.transform.position.x > GameManager.instance.playerUnitController.rimitPos[1] &&
            target.transform.position.y > GameManager.instance.playerUnitController.rimitPos[2] && target.transform.position.y < GameManager.instance.playerUnitController.rimitPos[3])
            {
                //�������� �÷��̾� ���� ������ ���
                if(GameManager.instance.MouseRayCast("PlayerUnit") != null)
                {
                    var player = GameManager.instance.MouseRayCast("PlayerUnit").GetComponent<PlayerUnitManager>();
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

                    if (player.SetItem((int)target.itemType))
                        Destroy(target.gameObject);
                }
                isTouch = false;
                touchDelay = 0;
            }

            //�������� ���� �ۿ� ���� ���
            else 
            {
                //�������� ���� ���� �÷��̾� ���� ������ ���
                if (GameManager.instance.MouseRayCast("PlayerUnit") != null)
                {
                    var player = GameManager.instance.MouseRayCast("PlayerUnit").GetComponent<PlayerUnitManager>();
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

                    if (player.SetItem((int)target.itemType))
                        Destroy(target.gameObject);

                    isTouch = false;
                    touchDelay = 0;
                }
                //�׳� ���� �ۿ� ���� ���
                else
                {
                    touchDelay = 0;
                    isTouch = false;
                    //�����ص� ���� ��ġ�� �̵�
                    target.transform.position = savePosition;
                    GameManager.instance.levelManager.SetErrorMessage("���� �ۿ� �������� ���� �� �����ϴ�!");
                }

            }
        }
    }
}
