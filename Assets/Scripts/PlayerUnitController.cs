using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PlayerUnitController : MonoBehaviour
{
    //유닛 터치상태 판단
    [Header("TouchStatus")]
    public bool isTouch;
    Vector2 mousePos;

    //터치한 유닛을 저장하기 위한 오브젝트
    [Header("TargetUnit")]
    PlayerUnitManager target;
    float touchDelay;

    //유닛 배치 가능한 포지션
    [Header("RimitPos, →←↓↑")]
    public float[] rimitPos;

    //맵 중앙에 표시되는 배치된 유닛의 수를 표시하는 텍스트
    [Header("ActiveUnitNumText")]
    [SerializeField] Text activeUnitNumText;

    //현재 배치된 유닛의 수
    [Header("CurrentActiveUnitNum")]
    [HideInInspector] public int currentActiveUnitNum;

    //배치 가능한 유닛의 최대 수
    const int maxActiveUnitNum = 9;

    Vector2 savePosition;

    

    private void Start()
    {
        activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

    }
    private void Update()
    {
        //전투 상태인 경우 리턴
        if (LevelManager.instance.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TouchPlayerUnit();

        if(isTouch)
            target.transform.position = mousePos;
    }

    /// <summary>
    /// 유닛을 배치하거나 판매
    /// </summary>
    void TouchPlayerUnit()
    {
        //터치상태가 아닐때 마우스 클릭 시
        if(Input.GetMouseButtonDown(0) && !isTouch)
        {
            //마우스 위치에 플레이어 유닛이 존재할 시
            if (MouseRayCast("Player", "PlayerUnit") != null)
            {
                //target에 플레이어 유닛 정보 저장
                target = MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();

                //target이 null이 아니면
                if (target != null)
                {
                    //터치 직전의 위치를 저장
                    savePosition = target.transform.position;
                    isTouch = true;
                    touchDelay = 0;
                    target.updownStatus = PlayerUnitManager.UpDownStatus.UP;
                }
            }
        }

        //터치상태일때 마우스 클릭 시
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
            //유닛이 Up상태일시
            if (target.updownStatus == PlayerUnitManager.UpDownStatus.UP)
            {
                //전장에 배치 가능한 수만큼 이미 몹이 배치되어있을 경우
                if (currentActiveUnitNum >= maxActiveUnitNum)
                {
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    isTouch = false;
                    //저장해둔 원래 위치로 이동
                    target.transform.position = savePosition;
                }
                //유닛을 전장 내에 놓았고, 현재 배치수가 최대 배치수보다 적을 경우
                if (target.transform.position.x < rimitPos[0] && target.transform.position.x > rimitPos[1] &&
                    target.transform.position.y > rimitPos[2] && target.transform.position.y < rimitPos[3] &&
                    currentActiveUnitNum < maxActiveUnitNum)
                {
                    //전장에 배치
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    //유닛이 있던 보관함을 비워준다
                    BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                    //활성화된 보관함의 수를 조정한다
                    BuySelectUnit.currentActiveUnitNum--;
                    //전장에 배치된 숫자를 조정한다
                    currentActiveUnitNum++;
                    //숫자를 조정했으니 텍스트도 변경해준다
                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                    isTouch = false;
                }
                //유닛을 전장 밖에 놓은 경우
                if (target.transform.position.x < rimitPos[1])
                {
                    //현재 마우스의 위치가 쓰레기통일 경우
                    if (MouseRayCast("SellBin", "SellBin") != null)
                    {
                        isTouch = false;
                        target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                        //유닛이 있던 보관함을 비워준다
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //활성화된 보관함의 수를 조정한다
                        BuySelectUnit.currentActiveUnitNum--;
                        //유닛 판매가만큼 골드를 얻는다
                        LevelManager.instance.SetGold(LevelManager.sellCost);
                        //유닛을 삭제한다
                        Destroy(target.gameObject);
                    }
                }
                //유닛을 집은 후 보관함으로 이동시키거나 보관함끼리 변경(제작예정)
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
