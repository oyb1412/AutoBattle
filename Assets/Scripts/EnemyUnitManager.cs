using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyUnitManager : UnitManager
{


    //스턴 타이머
    float stunTimer;
    //스턴 이펙트
    public ParticleSystem stunEffect;

    private void Start()
    {
        SetStatus(GameManager.instance.levelManager.currentRound);
    }
    public void SetStatus(int round)
    {
        var add = round - 1;
        attackDamage += add * 2;
        moveSpeed += 0.1f * add;
        attackSpeed -= 0.02f * add;
        maxHP += add * 15;
        currentHP = maxHP;
        transform.localScale = new Vector2(1f + (add * 0.05f), 1f + (round * 0.05f));
    }

    override protected void Update()
    {
        if (currentUnitState == unitState.DIE)
            return;

        if (GameManager.instance.levelManager.currentState == StateType.BATTLE)
        {
            base.MoveToTarget(LevelManager.playerLayer);
        }
        if (isStun)
        {
            stunTimer += Time.deltaTime;
            if(stunTimer > 1f)
            {
                isStun = false;
                stunTimer = 0;
            }
        }

        base.Update();
    }
}
