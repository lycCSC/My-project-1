using UnityEngine;

public class PlayerGeneratePoint : MonoBehaviour
{
    void Start()
    {
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefab/Player"), transform);
    }
}
