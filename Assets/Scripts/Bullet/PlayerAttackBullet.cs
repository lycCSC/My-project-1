using UnityEngine;

[RequireComponent(typeof(BulletProperty))]
public class PlayerAttackBullet : MonoBehaviour, IBulletDestroy
{
    BulletProperty bulletProperty;
    float bulletSurveTime;
    void Start()
    {
        bulletSurveTime = 0f;
        bulletProperty = GetComponent<BulletProperty>();
        bulletProperty.bulletSpeed = 10f;
        bulletProperty.bulletLife = 2f;
        bulletProperty.bulletAnimator = GetComponentInChildren<Animator>();
        bulletProperty.isBulletStopped = false;
        bulletProperty.destroySpeed = 3f;
        bulletProperty.bulletDamageRatio = 1f;
        bulletProperty.isTriggered = false;
    }

    void Update()
    {
        BulletLifeController();
        if (bulletProperty.bulletMoveType == BulletMoveType.Linear && !bulletProperty.isBulletStopped)
            BulletLineMove();
    }

    public void BulletLifeController()
    {
        bulletSurveTime += Time.deltaTime;
        if (bulletSurveTime >= bulletProperty.bulletLife)
            DestroyBullet();
    }

    public void BulletLineMove()
    {
        transform.Translate(Vector2.right * bulletProperty.bulletSpeed * bulletProperty.bulletDirection * Time.deltaTime);
    }

    public void DestroyBullet()
    {
        bulletSurveTime = 0f;
        bulletProperty.isBulletStopped = true;
        bulletProperty.bulletAnimator.SetFloat(BulletAnimationTransParamName.destroySpeed.ToString(), bulletProperty.destroySpeed);
        bulletProperty.bulletAnimator.SetBool(BulletAnimationTransParamName.isDestroy.ToString(), true);
    }
}
