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

    //터치한 유닛을 저장하기 위한 오브젝트
    [Header("TargetUnit")]
    PlayerUnitManager target;
    public float touchDelay;

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
            synageLevelText[i].text = synageLevel[0] + "레벨";
            synageNumText[i].text = activeUnitTypeNum[0] + "/ 9";
        }
    }
    private void Update()
    {
        //전투 상태인 경우 리턴
        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;

        TouchPlayerUnit();

        if(isTouch)
            target.transform.position = GameManager.instance.mousePos;
    }

    /// <summary>
    /// 유닛을 배치하거나 판매
    /// </summary>
    void TouchPlayerUnit()
    {
        //터치상태가 아니고, 현재 스테이트가 논 배틀 상태고, 아이템을 터치하지 않은 상태일때
        if(Input.GetMouseButtonDown(0) && !isTouch&& GameManager.instance.levelManager.currentState == StateType.NONBATTLE && !GameManager.instance.itemController.isTouch &&
            GameManager.instance.itemController.touchDelay > 0.1f)
        {
            //마우스 위치에 플레이어 유닛이 존재할 시
            if (GameManager.instance.MouseRayCast("Player", "PlayerUnit") != null)
            {
                //target에 플레이어 유닛 정보 저장
                target = GameManager.instance.MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();

                //target이 null이 아니면
                if (target != null)
                {
                    //터치 직전의 위치를 저장
                    savePosition = target.transform.position;

                    //보관함에 있는 유닛을 클릭 시
                    if (target.transform.position.x < rimitPos[1])
                        savePosition = target.transform.position;
                    else
                        savePosition = new Vector2(10f, 5f);
                    isTouch = true;
                    touchDelay = 0;
                }
            }
        }

        //터치상태일때 마우스 클릭 시
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
                //전장에 배치 가능한 수만큼 이미 몹이 배치되어있을 경우
                if (currentActiveUnitNum >= maxActiveUnitNum)
                {
                    isTouch = false;
                    //저장해둔 원래 위치로 이동
                    target.transform.position = savePosition;
                }
                //유닛을 전장 내에 놓았고, 현재 배치수가 최대 배치수보다 적을 경우
                if (target.transform.position.x < rimitPos[0] && target.transform.position.x > rimitPos[1] &&
                    target.transform.position.y > rimitPos[2] && target.transform.position.y < rimitPos[3] &&
                    currentActiveUnitNum < maxActiveUnitNum)
                {

                    //유닛의 상태를 wait로 변경
                    target.currentUnitState = unitState.WAIT;
                    //유닛이 있던 보관함을 비워준다
                    BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                    //활성화된 보관함의 수를 조정한다
                    BuySelectUnit.currentActiveUnitNum--;
                    //전장에 배치된 숫자를 조정한다
                    currentActiveUnitNum++;
                    //숫자를 조정했으니 텍스트도 변경해준다
                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                    SynageTextSet(1);
                    touchDelay = 0;
                    isTouch = false;
                }

                    //현재 마우스의 위치가 쓰레기통일 경우
                    if (GameManager.instance.MouseRayCast(LevelManager.SellBinTagNLayer, LevelManager.SellBinTagNLayer) != null)
                    {
                        touchDelay = 0;
                        isTouch = false;
                        //유닛이 있던 보관함을 비워준다
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //활성화된 보관함의 수를 조정한다
                        BuySelectUnit.currentActiveUnitNum--;
                        //유닛 판매가만큼 골드를 얻는다
                        GameManager.instance.levelManager.SetGold(LevelManager.sellCost);
                        //유닛을 삭제한다
                        target.currentUnitState = unitState.DIE;

                        //전장에서 쓰레기통으로 유닛을 옮긴 경우
                        if (savePosition.x > rimitPos[1])
                        {
                            SynageTextSet(-1);

                        }
                }
                    //현재 마우스의 위치가 보관함일 경우
                    if(GameManager.instance.MouseRayCast(LevelManager.SpawnColliderLayer) != null)
                    {
                        var col = GameManager.instance.MouseRayCast(LevelManager.SpawnColliderLayer);
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
                                        //전장에서 보관함으로 유닛을 옮긴 경우
                                        if (savePosition.x > rimitPos[1])
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
                                        //전장에서 보관함으로 유닛을 옮긴 경우
                                        if (savePosition.x > rimitPos[1])
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
                                        //전장에서 보관함으로 유닛을 옮긴 경우
                                        if (savePosition.x > rimitPos[1])
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

                                            //시너지 조정
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
                                        //전장에서 보관함으로 유닛을 옮긴 경우
                                        if (savePosition.x > rimitPos[1])
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
                                        //전장에서 보관함으로 유닛을 옮긴 경우
                                        if (savePosition.x > rimitPos[1])
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

    //시너지 텍스트 표시와 수치 변경(유닛을 배치할 때마다 실행)
    void SynageTextSet(int index)
    {
        //시너지 조정
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
                        synageLevelText[i].text = synageLevel[i] + "레벨";
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
