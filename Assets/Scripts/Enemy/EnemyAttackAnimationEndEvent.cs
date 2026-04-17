using UnityEngine;

public class EnemyAttackAnimationEndEvent : MonoBehaviour
{
    EnemyProperty enemyProperty;
    EnemyDecision enemyDecision;

    void Start()
    {
        enemyProperty = transform.GetComponentInParent<EnemyProperty>();
        enemyDecision = transform.GetComponentInParent<EnemyDecision>();
    }

    public void EndAttack()
    {
        if (enemyProperty.attackDierction > 0)
            enemyDecision.GetTargetAnimTransParamTrue(EnemyTransParam.isWalkRight);
        else
            enemyDecision.GetTargetAnimTransParamTrue(EnemyTransParam.isWalkLeft);
    }
}
