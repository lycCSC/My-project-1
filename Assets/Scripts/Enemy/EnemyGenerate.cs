using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyGenerate : MonoBehaviour
{
    public List<Transform> enemyGeneratePoints;
    int enemyInitializeNum = 5;
    float generateEnemyInterval = 1f;

    void Start()
    {
        foreach (GameObject enemyGeneratePoint in GameObject.FindGameObjectsWithTag(Tags.EnemyBornPoint.ToString()))
        {
            enemyGeneratePoints.Add(enemyGeneratePoint.transform);
        }
        StartCoroutine(GenerateEnemy());
    }

    IEnumerator GenerateEnemy()
    {
        while (true)
        {
            if(enemyInitializeNum <= 0)
            {
                yield break;
            }
            yield return new WaitForSeconds(generateEnemyInterval);
            EnemyPool.Instance.GetEnemy(EnemyType.CloseCombatLittleEnemy, enemyGeneratePoints[0]);
            enemyInitializeNum--;
        }
    }
}
