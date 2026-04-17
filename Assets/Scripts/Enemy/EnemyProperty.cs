using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperty : MonoBehaviour
{
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public bool isPlane;
    [HideInInspector]
    public float maxSpeed;
    public EnemyStatus enemyStatus;
    [HideInInspector]
    public Vector2 enemyPatrollingCenter;
    [HideInInspector]
    public Vector2 enemyPatrollingLeftPoint;
    [HideInInspector]
    public Vector2 enemyPatrollingRightPoint;
    [HideInInspector]
    public float enemyPatrolingDirection;
    [HideInInspector]
    public float patrolingRadius = 1f;
    [HideInInspector]
    public Animator enemyAnimator;
    [HideInInspector]
    public float pursuitDistance = 5f;
    [HideInInspector]
    public float damagedKockbackTime = 0.5f;
    [HideInInspector]
    public float maxDamagedKockbackTime = 0.5f;
    [HideInInspector]
    public float damagedKockbackSpeed = 1f;
    [HideInInspector]
    public float kockbackDirection;
    public EnemyStatus previousEnemyStatus;
    [HideInInspector]
    public Dictionary<EnemyStatus, Action> enemyStatusTransActionDic = new Dictionary<EnemyStatus, Action>();
    [HideInInspector]
    public float enemyRealDamagedValue;
    [HideInInspector]
    public float enemyHP = 20f;
    [HideInInspector]
    public float enemyMaxHP = 20f;
    [HideInInspector]
    public float enemyDefence = 5f;
    [HideInInspector]
    public bool isEnterSleepStatus = false;
    [HideInInspector]
    public bool isSleep;
    [HideInInspector]
    public bool isGroundEnemy;
    [HideInInspector]
    public float sleepTime = 0.8f;
    [HideInInspector]
    public float maxSleepTime = 0.8f;
    [HideInInspector]
    public bool isCanDamage = false;
    [HideInInspector]
    public float attackPower = 5f;
    [HideInInspector]
    public float attackTargetPosition = 1f;
    [HideInInspector]
    public float attackInterval = 2f;
    [HideInInspector]
    public bool isCanAttack = true;
    [HideInInspector]
    public float attackDierction = 1f;


    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        speed = 1f;
        InitEnemyStatusTransActionDic();
        kockbackDirection = 1f;
        isEnterSleepStatus = false;
        isSleep = false;
        RebornEnemy();
    }

    public void RebornEnemy()
    {
        enemyHP = enemyMaxHP;
        isEnterSleepStatus = false;
        isSleep = false;
        sleepTime = maxSleepTime;
        damagedKockbackSpeed = 1f;
        isGroundEnemy = false;
        TransToTargetEnemyStatus(EnemyStatus.Patrolling);
    }

    public void InitEnemyStatusTransActionDic()
    {
        enemyStatusTransActionDic.Add(EnemyStatus.Patrolling, SetEnemyPatrolling);
        enemyStatusTransActionDic.Add(EnemyStatus.Pursite, SetEnemyPursuit);
        enemyStatusTransActionDic.Add(EnemyStatus.Attack, SetEnemyAttack);
        enemyStatusTransActionDic.Add(EnemyStatus.Damaged, SetEnemyDamaged);
        enemyStatusTransActionDic.Add(EnemyStatus.Sleep, SetEnemySleep);
    }

    public void TransToTargetEnemyStatus(EnemyStatus targetEnemyStatus)
    {
        if (enemyStatusTransActionDic.ContainsKey(targetEnemyStatus))
        {
            if (isSleep && targetEnemyStatus != EnemyStatus.Sleep)
            {
                return;
            }
            enemyStatusTransActionDic[targetEnemyStatus].Invoke();
        }
    }

    public void SetEnemyPatrolling()
    {
        enemyStatus = EnemyStatus.Patrolling;
        enemyPatrolingDirection = 1f;
        enemyPatrollingCenter = transform.position;
        enemyPatrollingLeftPoint = new Vector2(enemyPatrollingCenter.x - patrolingRadius, enemyPatrollingCenter.y);
        enemyPatrollingRightPoint = new Vector2(enemyPatrollingCenter.x + patrolingRadius, enemyPatrollingCenter.y);
    }

    public void SetEnemyPursuit()
    {
        enemyAnimator.SetBool(EnemyTransParam.isAttackLeft.ToString(), false);
        enemyAnimator.SetBool(EnemyTransParam.isAttackRight.ToString(), false);
        enemyStatus = EnemyStatus.Pursite;
    }

    public void SetEnemyAttack()
    {
        enemyStatus = EnemyStatus.Attack;
    }

    public void SetEnemyDamaged()
    {
        damagedKockbackTime = maxDamagedKockbackTime;
        if (enemyStatus != EnemyStatus.Damaged)
        {
            previousEnemyStatus = enemyStatus;
        }
        enemyStatus = EnemyStatus.Damaged;
        GiveDamage();
    }

    public void SetEnemySleep()
    {
        enemyStatus = EnemyStatus.Sleep;
    }
    
    void GiveDamage()
    {
        enemyHP -= enemyRealDamagedValue - enemyDefence;
        if (enemyHP <= 0)
        {
            SetEnemySleep();
        }
    }
}
