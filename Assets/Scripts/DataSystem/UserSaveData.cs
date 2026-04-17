using System;
using System.IO;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UserSaveData : MonoBehaviour
{
    public float score;
    [HideInInspector]
    public int userID;
    [HideInInspector]
    public String userName;
    static UserSaveData instance;

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

    public static UserSaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<UserSaveData>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UserSaveData");
                    instance = obj.AddComponent<UserSaveData>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}