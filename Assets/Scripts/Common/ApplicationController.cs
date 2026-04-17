using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
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

    static ApplicationController instance;

    public static ApplicationController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<ApplicationController>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("ApplicationController");
                    instance = obj.AddComponent<ApplicationController>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}
