using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("BulletVelocity")]
    //불렛의 속력
    [SerializeField] float bulletVelocity;

    //불렛의 데미지
    [HideInInspector]public float bulletDamage;

    //적,아군 불렛 구별용 배열
    [HideInInspector]public groupType GroupType;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 불렛을 초기화 함과 동시에 발사
    /// </summary>
    /// <param name="targetpos">불렛 발사대상 포지션</param>
    /// <param name="dir">불렛 발사대상과 나와의 거리벡터</param>
    public void Init(Vector2 targetpos, Vector2 dir, float damage, groupType type)
    {
        float angle = Mathf.Atan2(targetpos.y - transform.position.y, targetpos.x - transform.position.x) * Mathf.Rad2Deg; 
        bulletDamage = damage;
        GroupType = type;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        rigid.velocity = dir.normalized * bulletVelocity;
    }
}
