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
            //�����ϴ� ������ �ϻ��ڰ�, ������ 2 �̻��ΰ��
            if (player.playerUnitType == playerUnitType.MELEE2 && player.level > 1)
            {
                //������ ����� ũ��Ƽ�� Ȯ�� ����
                var ran = Random.Range(0, (5 - player.level));
                if(ran == 0)
                {
                    //ũ��Ƽ�� �ߵ��� ������ 2��
                    critical = player.attackDamage;
                }
            }



            var target = collision.GetComponent<EnemyUnitManager>();

            //�����ϴ� ������ ���к��̰�, ������ 2 �̻��� ���
            if (player.playerUnitType == playerUnitType.MELEE0 && player.level > 1)
            {
                //������ ����� ���� Ȯ�� ����
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
