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

            //�и� �ó��� ���� ����
            var meleeSynage = GameManager.instance.playerUnitController.synageLevel[0];
            //�и� �ó��� ������ 0 �̻��̰�, ������ ���� �и� �����϶� �ݻ�
            if(meleeSynage > 0 && target.currentUnitType == unitType.MELEE)
            {
                //�ݻ� �������� ���� �������� 1% * �ó��� ��ġ(8,16,24)
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
