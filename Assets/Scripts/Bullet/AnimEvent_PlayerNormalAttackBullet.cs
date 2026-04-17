using UnityEngine;

public class AnimEvent_PlayerNormalAttackBullet : MonoBehaviour
{
    BulletProperty bulletProperty;
    
    void Start()
    {
        bulletProperty = GetComponentInParent<BulletProperty>();
    }
    public void Event_Bullet_Destory()
    {
        bulletProperty.bulletAnimator.SetBool(BulletAnimationTransParamName.isDestroy.ToString(), false);
        bulletProperty.bulletAnimator.Rebind();
        bulletProperty.bulletAnimator.Update(0);
        BulletPool.Instance.AddBullet(bulletProperty.bulletsType, gameObject.transform.parent.gameObject);
    }
}
