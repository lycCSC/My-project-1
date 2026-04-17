using System.Collections.Generic;
using UnityEngine;

public class HUDPool : MonoBehaviour
{
    public Dictionary<HUDType, Queue<GameObject>> hudPoolDic = new Dictionary<HUDType, Queue<GameObject>>();

    public void AddToPool(HUDType type, GameObject hud)
    {
        if (!hudPoolDic.ContainsKey(type))
        {
            hudPoolDic.Add(type, new Queue<GameObject>());
        }
        hudPoolDic[type].Enqueue(hud);
        hud.SetActive(false);
    }

    public GameObject GetFromPool(HUDType type)
    {
        if (hudPoolDic.ContainsKey(type) && hudPoolDic[type].Count > 0)
        {
            GameObject hud = hudPoolDic[type].Dequeue();
            hud.SetActive(true);
            return hud;
        }
        return null;
    }
}
