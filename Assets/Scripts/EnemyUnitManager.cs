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
    override protected void Update()
    {
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
