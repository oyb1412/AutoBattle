using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("BulletVelocity")]
    //불렛의 속력
    [SerializeField] float bulletVelocity;

    //불렛의 데미지
    [HideInInspector]public float bulletDamage;

    //적,아군 불렛 구별용 배열
    [HideInInspector]public unitGroup unitGroup;

    Rigidbody2D rigid;
    [SerializeField] ParticleSystem attackEffect;

    //발사 유닛 타입 저장
    playerUnitType unitType;

    //발사 유닛 레벨 저장
    int level;

    int count;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        count = 1;
    }

    /// <summary>
    /// 불렛을 초기화 함과 동시에 발사
    /// </summary>
    /// <param name="targetpos">불렛 발사대상 포지션</param>
    /// <param name="dir">불렛 발사대상과 나와의 거리벡터</param>
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
                //발사한 유닛이 스나이퍼고 레벨이 2 이상일 때
                if (unitType == playerUnitType.RANGE2 && level > 1)
                {
                    //레벨에 비례해 적이 즉사하도록 조절
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
                //발사한 유닛이 레인저고 레벨이 2 이상일 때
                if(unitType == playerUnitType.RANGE1 && level > 1)
                {
                    //레벨에 비례해 화살이 관통하도록 조절
                    var ran = Random.Range(0, level);
                    if (ran == 0)
                    {
                        //확률에 따라 화살이 바로 삭제되지 않도록 변경
                        Destroy(gameObject);
                    }
                }
                //아닌 경우 화살 바로 삭제
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
