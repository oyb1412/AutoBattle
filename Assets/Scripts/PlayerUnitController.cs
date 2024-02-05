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

                    //유닛을 보관함에서 전장으로 옮겼을 경우에만
                    if(savePosition.x < rimitPos[1])
                    {
                        //유닛이 있던 보관함을 비워준다
                        BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                        //활성화된 보관함의 수를 조정한다
                        BuySelectUnit.currentActiveUnitNum--;
                        //전장에 배치된 숫자를 조정한다
                        currentActiveUnitNum++;
                        //숫자를 조정했으니 텍스트도 변경해준다
                        activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                        SynageSet(1);
                    }
                    //유닛의 상태를 wait로 변경
                    target.currentUnitState = unitState.WAIT;
 
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

                        //유닛이 아이템을 가진 상태면 아이템 드랍
                        target.ClearItem();
                        //유닛이 지닌 모든 오브젝트 삭제
                        target.DeleteOtherObject();
                        //유닛을 삭제한다
                        target.currentUnitState = unitState.DIE;

                        //전장에서 쓰레기통으로 유닛을 옮긴 경우
                        if (savePosition.x > rimitPos[1])
                        {
                            SynageSet(-1);

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
                                            SynageSet(-1);
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
                                            SynageSet(-1);

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
                                            SynageSet(-1);
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

                                            SynageSet(-1);

                                            
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
    public void SynageSet(int index)
    {
        //전장에 배치하거나 해제한 유닛의 타입에 따라 시너지 숫자 변경
        switch (target.currentUnitType)
        {
            case unitType.MELEE:
                activeUnitTypeNum[(int)unitType.MELEE] += index;
                break;
            case unitType.RANGE:
                activeUnitTypeNum[(int)unitType.RANGE] += index;
                break;
            case unitType.MAGE:
                activeUnitTypeNum[(int)unitType.MAGE] += index;
                break;
        }
        //시너지 숫자 변경에 따른 텍스트 업데이트
        for(int i = 0;i<3;i++)
        {
            synageLevel[i] = activeUnitTypeNum[i] / 3;
                        synageLevelText[i].text = synageLevel[i] + "레벨";
            synageNumText[i].text = activeUnitTypeNum[i] + "/ 9";
        }
        
            var unit = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0;i<unit.Length;i++)
            {
                var player = unit[i].GetComponent<PlayerUnitManager>();

                if (player.currentUnitType == unitType.MELEE)
                {
                    switch (synageLevel[(int)unitType.MELEE])
                    {
                        case 0:
                             player.attackDamage = player.saveAttackDamage;
                        break;
                        case 1:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MELEE].upAttackDamage[1]));
                        break;
                        case 2:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MELEE].upAttackDamage[2]));
                            break;
                        case 3:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MELEE].upAttackDamage[3]));
                            break;
                    }
                }
                else if(player.currentUnitType == unitType.RANGE)
                {
                    switch (synageLevel[(int)unitType.RANGE])
                    {
                         case 0:
                            player.attackSpeed = player.saveAttackSpeed;
                            player.attackRange = player.saveAttackRange;
                            break;
                        case 1:
                            player.attackSpeed = player.saveAttackSpeed * (1 +  (0.01f * data[(int)unitType.RANGE].upAttackSpeed[1]));
                            player.attackRange = player.saveAttackRange * (1 + (0.01f * data[(int)unitType.RANGE].upAttackRange[1]));
                            break;
                        case 2:
                            player.attackSpeed = player.saveAttackSpeed * (1 + (0.01f * data[(int)unitType.RANGE].upAttackSpeed[2]));
                            player.attackRange = player.saveAttackRange * (1 +  (0.01f * data[(int)unitType.RANGE].upAttackRange[2]));
                            break;
                        case 3:
                            player.attackSpeed = player.saveAttackSpeed * (1 +  (0.01f * data[(int)unitType.RANGE].upAttackSpeed[3]));
                            player.attackRange = player.saveAttackRange * (1 + (0.01f * data[(int)unitType.RANGE].upAttackRange[3]));
                            break;
                    }
                }
                else
                {
                    switch (synageLevel[(int)unitType.MAGE])
                    {
                         case 0:
                            player.attackDamage = player.saveAttackDamage;
                            break;
                         case 1:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MAGE].downResistance[1]));
                            break;
                        case 2:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MAGE].downResistance[2]));
                            break;
                        case 3:
                            player.attackDamage = player.saveAttackDamage * (1 + (0.01f * data[(int)unitType.MAGE].downResistance[3]));
                            break;
                    }
                }
            }

        
    }





}
