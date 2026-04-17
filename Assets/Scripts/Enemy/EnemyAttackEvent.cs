using NUnit.Framework;
using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    PlayerAction playerAction;
    EnemyProperty enemyProperty;
    EnemyDecision enemyDecision;
    void Start()
    {
        enemyProperty = transform.GetComponentInParent<EnemyProperty>();
        playerAction = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<PlayerAction>();
        enemyDecision = transform.GetComponentInParent<EnemyDecision>();
    }

    public void Event_Animation_Attack()
    {
        if (enemyDecision.playerTransform == null || !PlayerProperty.Instance.isCanBeAttacked) return;
        float olderAttackTargetPosition = enemyProperty.attackTargetPosition;
        enemyProperty.attackTargetPosition = enemyDecision.playerTransform.position.x - transform.position.x > 0 ? 1f : -1f;
        if (olderAttackTargetPosition != enemyProperty.attackTargetPosition || !enemyProperty.isCanDamage)
        {
            return;
        }
        float direction = enemyDecision.playerTransform.position.x - transform.position.x > 0 ? 1f : -1f;
        enemyProperty.attackPower = 10f;
        PlayerProperty.Instance.debugTextForCheckHit.gameObject.SetActive(true);
        playerAction.PlayerDamaged(enemyProperty.attackPower, direction);
    }
}
