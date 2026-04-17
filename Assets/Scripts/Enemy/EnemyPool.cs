using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public Dictionary<EnemyType, Queue<GameObject>> enemyPoolDic = new Dictionary<EnemyType, Queue<GameObject>>();

    void Start()
    {
        foreach (EnemyType type in System.Enum.GetValues(typeof(EnemyType)))
        {
            enemyPoolDic[type] = new Queue<GameObject>();
        }
    }

    public void GetEnemy(EnemyType type, Transform bornPos)
    {
        if (enemyPoolDic.ContainsKey(type) && enemyPoolDic[type].Count > 0)
        {
            var newEnemy = enemyPoolDic[type].Dequeue();
            newEnemy.SetActive(true);
            newEnemy.transform.position = bornPos.position;
        }
        else
        {
            GameObject enemyInstance = Resources.Load<GameObject>(String.Format("Prefab/{0}", type.ToString()));
            GameObject.Instantiate(enemyInstance, bornPos.position, Quaternion.identity);
        }
    }

    public void AddEnemy(EnemyType type, GameObject enemy)
    {
        if (!enemyPoolDic.ContainsKey(type))
        {
            enemyPoolDic[type] = new Queue<GameObject>();
        }
        enemy.SetActive(false); // Deactivate the enemy before adding it back to the pool
        enemyPoolDic[type].Enqueue(enemy);
    }

    static EnemyPool instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static EnemyPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<EnemyPool>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemyPool");
                    instance = obj.AddComponent<EnemyPool>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}
