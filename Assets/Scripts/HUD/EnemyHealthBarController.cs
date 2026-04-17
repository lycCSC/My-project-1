using UnityEngine;

public class EnemyHealthBarController : MonoBehaviour
{
    UnityEngine.UI.Image healthBarImage;
    RectTransform rectTransform;

    Transform target;
    Vector3 offset;
    Camera cam;

    void Awake()
    {
        healthBarImage = GetComponent<UnityEngine.UI.Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    // 在敌人 Instantiate 后调用，传入敌人 Transform 和头顶偏移
    public void Initialize(Transform enemyTransform, Vector3 headOffset)
    {
        target = enemyTransform;
        offset = headOffset;
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 将敌人世界坐标转换为屏幕坐标，直接赋给 RectTransform（Screen Space - Overlay）
        rectTransform.position = cam.WorldToScreenPoint(target.position + offset);
    }

    public void ChangeHealthBar(float changeTime, float oldHP, float newHP, float maxHP)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", oldHP / maxHP,
            "to", newHP / maxHP,
            "time", changeTime,
            "onupdate", "OnUpdateHealthBar"));
    }

    public void OnUpdateHealthBar(float value)
    {
        healthBarImage.fillAmount = value;
    }
}
