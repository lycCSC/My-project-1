using UnityEngine;

[RequireComponent(typeof(SpecialEffectProperty))]
public class SpecialEffectPlay : MonoBehaviour
{
    SpecialEffectProperty effectProperty;

    void Start()
    {
        effectProperty = GetComponent<SpecialEffectProperty>();
        effectProperty.animator = GetComponent<Animator>();
        effectProperty.sleepEffectTime = effectProperty.maxSleepEffectTime;
    }

    void Update()
    {
        SleepEffectPlay();
    }

    void SleepEffectPlay()
    {
        if (effectProperty.sleepEffectTime > 0)
        {
            effectProperty.sleepEffectTime -= Time.deltaTime;
        }
        else
        {
            EnemySpecialEffect.Instance.AddSpecialEffect(EnemySpecialEffectType.Sleep, gameObject);
        }
    }
}
