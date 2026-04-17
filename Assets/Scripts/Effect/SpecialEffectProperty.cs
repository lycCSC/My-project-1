using UnityEngine;

public class SpecialEffectProperty : MonoBehaviour
{
    public float sleepEffectTime;
    public float maxSleepEffectTime = 1f;
    public Animator animator;
    public Quaternion generateEffectRotation = Quaternion.Euler(0, 0, 90);
}
