using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeUnitAttackCheck : MonoBehaviour
{
    [SerializeField] PlayerUnitManager player;
    [SerializeField]int count;

    private void Start()
    {
        count = player.count;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && count > 0)
        {
            float critical = 0;
            //공격하는 유닛이 암살자고, 레벨이 2 이상인경우
            if (player.playerUnitType == playerUnitType.MELEE2 && player.level > 1)
            {
                //레벨에 비례해 크리티컬 확률 조정
                var ran = Random.Range(0, (5 - player.level));
                if(ran == 0)
                {
                    //크리티컬 발동시 데미지 2배
                    critical = player.attackDamage;
                }
            }



            var target = collision.GetComponent<EnemyUnitManager>();

            //공격하는 유닛이 방패병이고, 레벨이 2 이상인 경우
            if (player.playerUnitType == playerUnitType.MELEE0 && player.level > 1)
            {
                //레벨에 비례해 스턴 확률 조정
                var ran = Random.Range(0, (10 - player.level));
                if (ran == 0)
                {
                    StartCoroutine(StunCorutine(target,1f));
                }
            }

            target.SetHP(-(player.attackDamage + critical));
            GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.MELEE);

            count--;


            if (count == 0)
                player.attackCollider.gameObject.SetActive(false);
        }
    }

    IEnumerator StunCorutine(EnemyUnitManager unit, float time)
    {
        unit.stunEffect.Play();
        unit.isStun = true;
        yield return new WaitForSeconds(time);
        unit.isStun = false;

        unit.stunEffect.Stop();
    }
    private void OnEnable()
    {
        count = player.count;
    }
}
