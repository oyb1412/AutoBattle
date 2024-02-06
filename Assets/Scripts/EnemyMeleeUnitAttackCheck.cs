using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyMeleeUnitAttackCheck : MonoBehaviour
{
    [SerializeField]EnemyUnitManager enemy;
    [SerializeField]int count;
    private void Start()
    {
        count = enemy.count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && count > 0)
        {
            var target = collision.GetComponent<PlayerUnitManager>();
            target.SetHP(-enemy.attackDamage);
            count--;
            if (count == 0)
                enemy.attackCollider.gameObject.SetActive(false);

            //밀리 시너지 레벨 저장
            var meleeSynage = GameManager.instance.playerUnitController.synageLevel[0];
            //밀리 시너지 레벨이 0 이상이고, 공격한 적이 밀리 유닛일때 반사
            if(meleeSynage > 0 && target.currentUnitType == unitType.MELEE)
            {
                //반사 데미지는 유닛 데미지의 1% * 시너지 수치(8,16,24)
                var damage = enemy.attackDamage * 0.01f * meleeSynage;
                enemy.SetHP(damage);
                GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.MELEE);

            }
        }
    }

    private void OnEnable()
    {
        count = enemy.count;
    }
}
