using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("BulletVelocity")]
    //�ҷ��� �ӷ�
    [SerializeField] float bulletVelocity;

    //�ҷ��� ������
    [HideInInspector]public float bulletDamage;

    //��,�Ʊ� �ҷ� ������ �迭
    [HideInInspector]public unitGroup unitGroup;

    Rigidbody2D rigid;
    [SerializeField] ParticleSystem attackEffect;

    //�߻� ���� Ÿ�� ����
    playerUnitType unitType;

    //�߻� ���� ���� ����
    int level;

    int count;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        count = 1;
    }

    /// <summary>
    /// �ҷ��� �ʱ�ȭ �԰� ���ÿ� �߻�
    /// </summary>
    /// <param name="targetpos">�ҷ� �߻��� ������</param>
    /// <param name="dir">�ҷ� �߻���� ������ �Ÿ�����</param>
    public void Init(Vector2 targetpos, Vector2 dir, float damage, unitGroup type, int level = 0, playerUnitType unittype = 0)
    {
        float angle = Mathf.Atan2(targetpos.y - transform.position.y, targetpos.x - transform.position.x) * Mathf.Rad2Deg; 
        bulletDamage = damage;
        unitGroup = type;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        rigid.velocity = dir.normalized * bulletVelocity;
        unitType = unittype;
        this.level = level;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitGroup == unitGroup.PLAYER)
        {
            if (collision.CompareTag("Enemy"))
            {
                float critical = 0;
                //�߻��� ������ �������۰� ������ 2 �̻��� ��
                if (unitType == playerUnitType.RANGE2 && level > 1)
                {
                    //������ ����� ���� ����ϵ��� ����
                    var ran = Random.Range(0, 20 - level * 2);
                    if (ran == 0)
                    {
                        critical = 300f;
                    }
                }
                var target = collision.GetComponent<EnemyUnitManager>();
                target.SetHP(-(bulletDamage + critical));
                var effect = Instantiate(attackEffect, null);
                effect.transform.position = transform.position;
                effect.Play();
                Destroy(effect.gameObject, 0.5f);
                //�߻��� ������ �������� ������ 2 �̻��� ��
                if(unitType == playerUnitType.RANGE1 && level > 1)
                {
                    //������ ����� ȭ���� �����ϵ��� ����
                    var ran = Random.Range(0, level);
                    if (ran == 0)
                    {
                        //Ȯ���� ���� ȭ���� �ٷ� �������� �ʵ��� ����
                        Destroy(gameObject);
                    }
                }
                //�ƴ� ��� ȭ�� �ٷ� ����
                else
                    Destroy(gameObject);
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                var target = collision.GetComponent<PlayerUnitManager>();
                target.SetHP(-bulletDamage);
                var effect = Instantiate(attackEffect, null);
                effect.transform.position = transform.position;
                effect.Play();
                Destroy(effect.gameObject, 0.5f);
                Destroy(gameObject);
            }
        }

    }
}
