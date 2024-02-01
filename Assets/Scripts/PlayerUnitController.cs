using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
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
    //보관함 위치를 특정하기 위한 오브젝트
    [SerializeField] BuySelectUnit buySelectUnit;

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
                    if (MouseRayCast(LevelManager.SellBinTagNLayer, LevelManager.SellBinTagNLayer) != null)
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
                    //현재 마우스의 위치가 보관함일 경우
                    if(MouseRayCast(LevelManager.SpawnColliderLayer) != null)
                    {
                        var col = MouseRayCast(LevelManager.SpawnColliderLayer);
                        //보관함이 null이 아닐때
                        if (col != null)
                        {
                            //유닛이 있던 보관함을 비워준다
                            BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                            //선택한 보관함을 태그로 구별
                            switch (col.tag)
                            {
                                case LevelManager.SpawnColliderTag0:
                                    if(BuySelectUnit.summonIndex[0] == false)
                                    {
                                        target.buyUnitIndex = 0;
                                        BuySelectUnit.summonIndex[0] = true;
                                        target.transform.position = buySelectUnit.summonPos[0];
                                    }
                                    break;                            
                                case LevelManager.SpawnColliderTag1:
                                    if (BuySelectUnit.summonIndex[1] == false)
                                    {
                                        target.buyUnitIndex = 1;
                                        BuySelectUnit.summonIndex[1] = true;
                                        target.transform.position = buySelectUnit.summonPos[1];
                                    }
                                    break;                             
                                case LevelManager.SpawnColliderTag2:
                                    if(BuySelectUnit.summonIndex[2] == false)
                                    {
                                        target.buyUnitIndex = 2;
                                        BuySelectUnit.summonIndex[2] = true;
                                        target.transform.position = buySelectUnit.summonPos[2];
                                    }
                                    break;                              
                                case LevelManager.SpawnColliderTag3:
                                    if(BuySelectUnit.summonIndex[3] == false)
                                    {
                                        target.buyUnitIndex = 3;
                                        BuySelectUnit.summonIndex[3] = true;
                                        target.transform.position = buySelectUnit.summonPos[3];
                                    }
                                    break;                            
                                case LevelManager.SpawnColliderTag4:
                                    if(BuySelectUnit.summonIndex[4] == false)
                                    {
                                        target.buyUnitIndex = 4;
                                        BuySelectUnit.summonIndex[4] = true;
                                        target.transform.position = buySelectUnit.summonPos[4];
                                    }
                                    break;
                            }
                            isTouch = false;
                            target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                            
                            //전장에서 보관함으로 유닛을 옮긴 경우
                            if(savePosition.x < rimitPos[1])
                            {
                                //보관함이 꽉 차지 않은 경우
                                if (currentActiveUnitNum < maxActiveUnitNum)
                                {
                                    //활성화된 보관함의 수를 조정한다
                                    BuySelectUnit.currentActiveUnitNum++;
                                    //전장에 배치된 숫자를 조정한다
                                    currentActiveUnitNum--;
                                    //숫자를 조정했으니 텍스트도 변경해준다
                                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);
                                }
                            }
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
