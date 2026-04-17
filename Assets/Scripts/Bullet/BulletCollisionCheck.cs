using UnityEngine;

public class BulletCollisionCheck : MonoBehaviour
{
    public IBulletDestroy bulletDestroy;
    public BulletProperty bulletProperty;

    private void Start()
    {
        bulletDestroy = GetComponentInParent<IBulletDestroy>();
        bulletProperty = GetComponentInParent<BulletProperty>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (bulletProperty.isTriggered) return;
        if (col.gameObject.tag == Tags.Ground.ToString() || col.gameObject.tag == Tags.EnemyCanBeAttacked.ToString())
        {
            if (col.gameObject.tag == Tags.EnemyCanBeAttacked.ToString())
            {
                if (bulletProperty.firePoint.position.x > col.gameObject.transform.position.x)
                {
                    col.gameObject.GetComponent<EnemyProperty>().kockbackDirection = -1f;
                }
                else
                {
                    col.gameObject.GetComponent<EnemyProperty>().kockbackDirection = 1f;
                }
                col.gameObject.GetComponent<EnemyProperty>().enemyRealDamagedValue =
                bulletProperty.bulletRealDamage * bulletProperty.bulletDamageRatio;
                col.gameObject.GetComponent<EnemyProperty>().TransToTargetEnemyStatus(EnemyStatus.Damaged);
                bulletProperty.isTriggered = true;
            }
            bulletDestroy?.DestroyBullet();
        }
    }
}
