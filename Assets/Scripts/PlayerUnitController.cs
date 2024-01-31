using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PlayerUnitController : MonoBehaviour
{
    [Header("TouchStatus")]
    public bool isTouch;
    Vector2 mousePos;

    [Header("TargetUnit")]
    PlayerUnitManager target;
    float touchDelay;

    [Header("RimitPos, ¡æ¡ç¡é¡è")]
    public float[] rimitPos;

    [Header("ActiveUnitNumText")]
    [SerializeField] Text activeUnitNumText;

    [Header("CurrentActiveUnitNum")]
    [HideInInspector] public int currentActiveUnitNum;

    Vector2 savePosition;

    const int maxActiveUnitNum = 9;
    

    private void Start()
    {
        activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

    }
    private void Update()
    {
        if (LevelManager.instance.currentState == StateType.BATTLE)
            return;

        touchDelay += Time.deltaTime;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TouchPlayerUnit();
        if(isTouch)
            target.transform.position = mousePos;
    }


    void TouchPlayerUnit()
    {
        if(Input.GetMouseButtonDown(0) && !isTouch)
        {
            if (MouseRayCast("Player", "PlayerUnit") != null)
            {
                target = MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();
                if (target != null)
                {
                    savePosition = target.transform.position;
                    isTouch = true;
                    touchDelay = 0;
                    target.updownStatus = PlayerUnitManager.UpDownStatus.UP;

                }
            }
        }
        if (Input.GetMouseButtonDown(0) && touchDelay > 0.1f && isTouch)
        {
            if (target.updownStatus == PlayerUnitManager.UpDownStatus.UP)
            {
                if (currentActiveUnitNum >= maxActiveUnitNum)
                {
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    isTouch = false;
                    target.transform.position = savePosition;
                }
                if (target.transform.position.x < rimitPos[0] && target.transform.position.x > rimitPos[1] &&
                    target.transform.position.y > rimitPos[2] && target.transform.position.y < rimitPos[3] &&
                    currentActiveUnitNum < maxActiveUnitNum)
                {
                    target.updownStatus = PlayerUnitManager.UpDownStatus.DOWN;
                    BuySelectUnit.summonIndex[target.buyUnitIndex] = false;
                    BuySelectUnit.currentActiveUnitNum--;
                    currentActiveUnitNum++;
                    activeUnitNumText.text = string.Format("{0} / {1}", currentActiveUnitNum, maxActiveUnitNum);

                    isTouch = false;
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
}
