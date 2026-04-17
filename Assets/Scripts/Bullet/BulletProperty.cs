using UnityEngine;

public class BulletProperty : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLife;
    public float bulletDirection;
    public BulletsType bulletsType;
    public BulletMoveType bulletMoveType;
    public Animator bulletAnimator;
    public Transform firePoint;
    public float bulletRealDamage;
    public float bulletDamageRatio;
    public bool isBulletStopped;
    public float destroySpeed = 1f;
    public bool isTriggered;
}
