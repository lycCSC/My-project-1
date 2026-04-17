using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyProperty))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDecision : MonoBehaviour
{
    EnemyProperty enemyProperty;
    Rigidbody2D enemyRB;
    [HideInInspector]
    public Transform playerTransform;
    private Dictionary<EnemyTransParam, int> enemyAnimTransParamHash = new();

    void Start()
    {
        enemyProperty = GetComponent<EnemyProperty>();
        enemyRB = GetComponent<Rigidbody2D>();
        enemyProperty.TransToTargetEnemyStatus(EnemyStatus.Patrolling);
        InitializeEnemyAnimTransParamHash();
    }

    void InitializeEnemyAnimTransParamHash()
    {
        foreach (EnemyTransParam param in System.Enum.GetValues(typeof(EnemyTransParam)))
        {
            enemyAnimTransParamHash[param] = Animator.StringToHash(param.ToString());
        }
    }

    void Update()
    {
        DecisionMaking();
    }

    void DecisionMaking()
    {
        if (enemyProperty.enemyStatus == EnemyStatus.Patrolling)
            Patrolling();
        else if (enemyProperty.enemyStatus == EnemyStatus.Pursite)
            PursuitPlayer();
        else if (enemyProperty.enemyStatus == EnemyStatus.Attack)
            AttackPlayer();
        else if (enemyProperty.enemyStatus == EnemyStatus.Damaged)
            Damaged();
        else if (enemyProperty.enemyStatus == EnemyStatus.Sleep)
            EnmeySleep();
    }

    void Damaged()
    {
        if (enemyProperty.damagedKockbackTime > 0)
        {
            enemyProperty.damagedKockbackTime -= Time.deltaTime;
        }
        else
        {
            enemyProperty.TransToTargetEnemyStatus(enemyProperty.previousEnemyStatus);
            return;
        }
        if (enemyProperty.kockbackDirection > 0)
        {
            GetTargetAnimTransParamTrue(EnemyTransParam.isDamagedLeft);
        }
        else
        {
            GetTargetAnimTransParamTrue(EnemyTransParam.isDamagedRight);
        }
        enemyRB.linearVelocity = new Vector2(enemyProperty.kockbackDirection * enemyProperty.damagedKockbackSpeed, enemyRB.linearVelocityY);
    }

    public void GetTargetAnimTransParamTrue(EnemyTransParam enemyTransParam)
    {
        foreach (var itemParameter in enemyAnimTransParamHash)
        {
            enemyProperty.enemyAnimator.SetBool(itemParameter.Value, itemParameter.Key == enemyTransParam);
        }
    }

    public void Patrolling()
    {
        if (enemyProperty.enemyPatrolingDirection > 0)
        {
            if (transform.position.x < enemyProperty.enemyPatrollingRightPoint.x)
            {
                enemyRB.linearVelocity = new Vector2(enemyProperty.speed, enemyRB.linearVelocityY);
                GetTargetAnimTransParamTrue(EnemyTransParam.isWalkRight);
            }
            else
            {
                enemyRB.linearVelocity = new Vector2(0, enemyRB.linearVelocityY);
                enemyProperty.enemyPatrolingDirection *= -1;
            }
        }
        else
        {
            if (transform.position.x > enemyProperty.enemyPatrollingLeftPoint.x)
            {
                GetTargetAnimTransParamTrue(EnemyTransParam.isWalkLeft);
                enemyRB.linearVelocity = new Vector2(-enemyProperty.speed, enemyRB.linearVelocityY);
            }
            else
            {
                enemyRB.linearVelocity = new Vector2(0, enemyRB.linearVelocityY);
                enemyProperty.enemyPatrolingDirection *= -1;
            }
        }
    }

    public void AttackPlayer()
    {
        if (!isPursuitOrAttackGoOn() || playerTransform == null || !enemyProperty.isCanAttack) return;
        enemyProperty.isCanAttack = false;
        enemyProperty.attackDierction = playerTransform.position.x > transform.position.x ? 1f : -1f;
        if (enemyProperty.attackDierction >= 0)
            GetTargetAnimTransParamTrue(EnemyTransParam.isAttackRight);
        else
            GetTargetAnimTransParamTrue(EnemyTransParam.isAttackLeft);
        enemyRB.linearVelocity = new Vector2(0, enemyRB.linearVelocityY);
        StartCoroutine(AttackIntervalCoroutine());
    }

    IEnumerator AttackIntervalCoroutine()
    {
        yield return new WaitForSeconds(enemyProperty.attackInterval);
        enemyProperty.isCanAttack = true;
    }

    public void PursuitPlayer()
    {
        if (!isPursuitOrAttackGoOn() || playerTransform == null) return;
        float direction = playerTransform.position.x > transform.position.x ? 1f : -1f;
        if (direction > 0)
            GetTargetAnimTransParamTrue(EnemyTransParam.isWalkRight);
        else
            GetTargetAnimTransParamTrue(EnemyTransParam.isWalkLeft);
        enemyRB.linearVelocity = new Vector2(direction * enemyProperty.speed, enemyRB.linearVelocityY);
    }

    bool isPursuitOrAttackGoOn()
    {
        if (PlayerProperty.Instance.invisible || enemyProperty.enemyStatus == EnemyStatus.Sleep) return false;
        else return true;
    }

    public void EnmeySleep()
    {
        if (enemyProperty.sleepTime > 0)
        {
            enemyProperty.sleepTime -= Time.deltaTime;
        }
        else
        {
            EnemySpecialEffect.Instance.GenerateSleepEffect(EnemySpecialEffectType.Sleep, transform);
            EnemyPool.Instance.AddEnemy(EnemyType.CloseCombatLittleEnemy, gameObject);
            return;
        }
        if (enemyProperty.isEnterSleepStatus)
        {
            enemyProperty.enemyAnimator.SetBool(EnemyTransParam.isSleep.ToString(), false);
            return;
        }
        GetTargetAnimTransParamTrue(EnemyTransParam.isSleep);
        enemyRB.linearVelocity = Vector2.zero;
        enemyProperty.isEnterSleepStatus = true;
    }
}
