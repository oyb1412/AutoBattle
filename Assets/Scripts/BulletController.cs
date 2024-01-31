using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("BulletVelocity")]
    //�ҷ��� �ӷ�
    [SerializeField] float bulletVelocity;

    //�ҷ��� ������
    [HideInInspector]public float bulletDamage;

    //��,�Ʊ� �ҷ� ������ �迭
    [HideInInspector]public groupType GroupType;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// �ҷ��� �ʱ�ȭ �԰� ���ÿ� �߻�
    /// </summary>
    /// <param name="targetpos">�ҷ� �߻��� ������</param>
    /// <param name="dir">�ҷ� �߻���� ������ �Ÿ�����</param>
    public void Init(Vector2 targetpos, Vector2 dir, float damage, groupType type)
    {
        float angle = Mathf.Atan2(targetpos.y - transform.position.y, targetpos.x - transform.position.x) * Mathf.Rad2Deg; 
        bulletDamage = damage;
        GroupType = type;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        rigid.velocity = dir.normalized * bulletVelocity;
    }
}
