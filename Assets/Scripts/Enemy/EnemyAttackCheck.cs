using UnityEngine;

public class EnemyAttackCheck : MonoBehaviour
{
    EnemyProperty enemyProperty;

    void Start()
    {
        enemyProperty = transform.GetComponentInParent<EnemyProperty>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            enemyProperty.TransToTargetEnemyStatus(EnemyStatus.Attack);
            enemyProperty.isCanDamage = true;
            enemyProperty.attackTargetPosition = collision.transform.position.x - transform.position.x > 0 ? 1f : -1f;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            enemyProperty.TransToTargetEnemyStatus(EnemyStatus.Pursite);
            enemyProperty.isCanDamage = false;
        }
    }
}
