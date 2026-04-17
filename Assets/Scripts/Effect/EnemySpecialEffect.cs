using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpecialEffect : MonoBehaviour
{
    public Dictionary<EnemySpecialEffectType, Queue<GameObject>> enemySpecialEffectsDic = new Dictionary<EnemySpecialEffectType, Queue<GameObject>>();

    public void AddSpecialEffect(EnemySpecialEffectType enemySpecialEffectType, GameObject gameObject)
    {
        if (!enemySpecialEffectsDic.ContainsKey(enemySpecialEffectType))
        {
            enemySpecialEffectsDic[enemySpecialEffectType] = new Queue<GameObject>();
        }
        else
        {
            if (enemySpecialEffectsDic.ContainsKey(enemySpecialEffectType))
            {
                enemySpecialEffectsDic[enemySpecialEffectType].Enqueue(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    public void GenerateSleepEffect(EnemySpecialEffectType enemySpecialEffectType, Transform generatePos)
    {
        if (enemySpecialEffectsDic.ContainsKey(enemySpecialEffectType) && enemySpecialEffectsDic[enemySpecialEffectType].Count > 0)
        {
            var gameObject = enemySpecialEffectsDic[enemySpecialEffectType].Dequeue();
            gameObject.SetActive(true);
            gameObject.transform.position = generatePos.position;
            gameObject.GetComponent<SpecialEffectProperty>().sleepEffectTime = gameObject.GetComponent<SpecialEffectProperty>().maxSleepEffectTime;
        }
        else
        {
            var gameObject = Resources.Load<GameObject>(String.Format("Prefab/{0}", EnemySpecialEffectName.SleepEffect.ToString()));
            var newgameObject = GameObject.Instantiate<GameObject>(gameObject, generatePos.position, Quaternion.identity);
            newgameObject.transform.rotation = newgameObject.GetComponent<SpecialEffectProperty>().generateEffectRotation;
            newgameObject.GetComponent<SpecialEffectProperty>().sleepEffectTime = newgameObject.GetComponent<SpecialEffectProperty>().maxSleepEffectTime;
        }
    }
    
    static EnemySpecialEffect instance;

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

    public static EnemySpecialEffect Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<EnemySpecialEffect>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemySpecialEffect");
                    instance = obj.AddComponent<EnemySpecialEffect>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}
