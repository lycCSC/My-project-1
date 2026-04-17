using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarController : MonoBehaviour
{
    Image healthBarImage;

    private void Start()
    {
        transform.Find("I_HealthBar").TryGetComponent<Image>(out healthBarImage);
    }

    public void UpdateHealthBarByITween(float changeTime, float oldHP, float newHP, float maxHP)
    {
        Debug.Log($"Updating health bar: oldHP={oldHP}, newHP={newHP}, maxHP={maxHP}");
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", oldHP / maxHP,
            "to", newHP / maxHP,
            "time", changeTime,
            "easetype", iTween.EaseType.linear,
            "onupdate", "OnUpdateRedBarFill"   // 每帧回调函数
        ));
    }

    public void OnUpdateRedBarFill(float value)
    {
        healthBarImage.fillAmount = value;
    }
}
