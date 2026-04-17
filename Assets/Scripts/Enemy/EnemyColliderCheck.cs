using System;
using UnityEngine;

public class EnemyColliderCheck : MonoBehaviour
{
    EnemyDecision EnemyDecision;
    EnemyProperty EnemyProperty;
    public LayerMask itselfLayer;

    void Start()
    {
        EnemyDecision = GetComponentInParent<EnemyDecision>();
        EnemyProperty = GetComponentInParent<EnemyProperty>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float distance = Mathf.Abs((transform.position.x - collision.transform.position.x) * 2);
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, collision.transform.position - transform.position, distance, itselfLayer);
            if (hit2D.transform.CompareTag("Player"))
            {
                EnemyDecision.playerTransform = collision.transform;
                EnemyProperty.TransToTargetEnemyStatus(EnemyStatus.Pursite);
            }
        }
    }
}
