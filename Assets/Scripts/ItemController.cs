using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ItemController : MonoBehaviour
{
    //아이템 터치 상태
    public bool isTouch;

    //아이템이 있던 위치
    Vector2 savePosition;

    //터치한 아이템
    [SerializeField] ItemManager target;

    //연속터치 제한을 위한 딜레이
    public float touchDelay;


    private void Update()
    {
        //전투 상태인 경우 리턴
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;

        TouchItem();

        if (isTouch)
            target.transform.position = GameManager.instance.mousePos;
    }
    /// <summary>
    /// 아이템 조작
    /// </summary>
    void TouchItem()
    {
        //터치상태가 아니고, 현재 스테이트가 논 배틀 상태고, 플레이어를 터치하지 않은 상태일때
        if (Input.GetMouseButtonDown(0) && !isTouch && GameManager.instance.levelManager.currentState == StateType.NONBATTLE && !GameManager.instance.playerUnitController.isTouch &&
            GameManager.instance.playerUnitController.touchDelay > 0.1f)
        {
            //마우스 위치에 아이템이 존재할 시
            if (GameManager.instance.MouseRayCast("Item") != null)
            {
                //target에 아이템 정보 저장
                target = GameManager.instance.MouseRayCast("Item").GetComponent<ItemManager>();

                //target이 null이 아니면
                if (target != null)
                {
                    //터치 직전의 위치를 저장
                    savePosition = target.transform.position;
                    isTouch = true;
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

                    touchDelay = 0;
                }
            }
        }

        //터치상태일때 마우스 클릭 시
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
            //아이템을 전장 내에 놓았을 경우
            if (target.transform.position.x < GameManager.instance.playerUnitController.rimitPos[0] && target.transform.position.x > GameManager.instance.playerUnitController.rimitPos[1] &&
            target.transform.position.y > GameManager.instance.playerUnitController.rimitPos[2] && target.transform.position.y < GameManager.instance.playerUnitController.rimitPos[3])
            {
                //아이템을 플레이어 에게 놓았을 경우
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

            //아이템을 전장 밖에 놓은 경우
            else 
            {
                //아이템을 전장 밖의 플레이어 에게 놓았을 경우
                if (GameManager.instance.MouseRayCast("PlayerUnit") != null)
                {
                    var player = GameManager.instance.MouseRayCast("PlayerUnit").GetComponent<PlayerUnitManager>();
                    GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.SELECT);

                    if (player.SetItem((int)target.itemType))
                        Destroy(target.gameObject);

                    isTouch = false;
                    touchDelay = 0;
                }
                //그냥 전장 밖에 놓은 경우
                else
                {
                    touchDelay = 0;
                    isTouch = false;
                    //저장해둔 원래 위치로 이동
                    target.transform.position = savePosition;
                    GameManager.instance.levelManager.SetErrorMessage("전장 밖에 아이템을 놓을 수 없습니다!");
                }

            }
        }
    }
}
