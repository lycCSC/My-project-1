using UnityEngine;

public class EnemyFootPointCheck : MonoBehaviour
{
    EnemyProperty enemyProperty;

    void Start()
    {
        enemyProperty = transform.GetComponentInParent<EnemyProperty>();
        enemyProperty.isGroundEnemy = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            enemyProperty.isGroundEnemy = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            enemyProperty.isGroundEnemy = false;
        }
    }
}
