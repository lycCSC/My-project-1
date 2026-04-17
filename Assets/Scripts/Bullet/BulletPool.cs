using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    Dictionary<BulletsType, Queue<GameObject>> bulletPool = new Dictionary<BulletsType, Queue<GameObject>>();
    BulletProperty bulletProperty;

    public void AddBullet(BulletsType bulletsType, GameObject gameObject)
    {
        if (!bulletPool.ContainsKey(bulletsType)) bulletPool.Add(bulletsType, new Queue<GameObject>());
        gameObject.SetActive(false);
        bulletPool[bulletsType].Enqueue(gameObject);
    }

    public void GetBullet(float playerAttackPower, Transform firePos, BulletMoveType bulletMoveType, BulletsType bulletsType, Vector2 startPos, float direction)
    { 
        GameObject bulletInstance;
        if (bulletPool.ContainsKey(bulletsType) && bulletPool[bulletsType].Count > 0)
        {
            bulletInstance = bulletPool[bulletsType].Dequeue();
            bulletInstance.SetActive(true);
            bulletInstance.transform.position = startPos;
            InitBullet(playerAttackPower, firePos, bulletInstance, bulletMoveType, bulletsType, direction);
        }
        else
        {
            if (!bulletPool.ContainsKey(bulletsType)) bulletPool.Add(bulletsType, new Queue<GameObject>());
            var tempBulletInstance = Resources.Load<GameObject>(String.Format("Prefab/{0}", bulletsType.ToString()));
            bulletInstance = GameObject.Instantiate<GameObject>(tempBulletInstance, startPos, Quaternion.identity);
            InitBullet(playerAttackPower, firePos, bulletInstance, bulletMoveType, bulletsType, direction);
        }
    }

    public void InitBullet(float playerAttackPower, Transform firePoint, GameObject bulletInstance, BulletMoveType bulletMoveType, BulletsType bulletsType, float direction)
    {
        bulletProperty = bulletInstance.GetComponent<BulletProperty>();
        bulletProperty.bulletsType = bulletsType;
        bulletProperty.bulletDirection = direction;
        bulletProperty.bulletMoveType = bulletMoveType;
        bulletProperty.isBulletStopped = false;
        bulletProperty.firePoint = firePoint;
        bulletProperty.bulletRealDamage = playerAttackPower;
        bulletProperty.isTriggered = false;
    }

    static BulletPool instance;

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

    public static BulletPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<BulletPool>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BulletPool");
                    instance = obj.AddComponent<BulletPool>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}
