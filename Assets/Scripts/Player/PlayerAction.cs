using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    public Button supriseButton;
    public Transform firePoint;
    float maxAttackTime = 0.5f;
    Rigidbody2D actionRigidbody2D;

    void Start()
    {
        var controllerUI = GameObject.FindGameObjectsWithTag(Tags.ControllerUI.ToString());
        firePoint = FindChildInAnyLayer.FindChildRecursive(transform, "FirePoint");
        actionRigidbody2D = GetComponent<Rigidbody2D>();
        foreach (var ui in controllerUI)
        {
            if (ui.name == "B_Suprise")
                supriseButton = ui.GetComponent<Button>();
        }
        supriseButton.onClick.AddListener(Event_B_Suprise);
        PlayerProperty.Instance.attackPower = 10f;
    }

    IEnumerator ForceEndAttackAnimation()
    {
        float timer = 0f;
        while (timer < maxAttackTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerProperty.Instance.isSupriseCovered = false;
    }


    void Update()
    {
        if (GameplayPauseController.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) Event_B_Suprise();
    }

    public void Event_B_Suprise()
    {
        if (GameplayPauseController.IsPaused) return;
        if (PlayerProperty.Instance.playerState != PlayerState.Side
        || PlayerProperty.Instance.isSupriseCovered
        || PlayerProperty.Instance.isControlLocked
        || PlayerProperty.Instance.isDashing) return;
        PlayerProperty.Instance.isSupriseCovered = true;
        StartCoroutine(ForceEndAttackAnimation());
        BulletPool.Instance.GetBullet(PlayerProperty.Instance.attackPower, firePoint, BulletMoveType.Linear, BulletsType.PlayerAttackBullet, firePoint.position, PlayerProperty.Instance.direction);
        PlayerProperty.Instance.ChangeStatus(PlayerStatus.suprise);
        PlayerProperty.Instance.attackSpeed = 3f;
        PlayerProperty.Instance.ChangeCurrentAnimation(PlayerFloatAnimationTransParam.attackSpeed, PlayerProperty.Instance.attackSpeed);
        PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isSuprise);
    }

    public void PlayerDamaged(float damage, float direction)
    {
        if (!PlayerProperty.Instance.isCanBeAttacked) return;
        PlayerProperty.Instance.debugTextForCheckHit.gameObject.SetActive(true);
        PlayerProperty.Instance.isCanBeAttacked = false;
        PlayerProperty.Instance.oldHpForAnim = PlayerProperty.Instance.hp;
        PlayerProperty.Instance.hp -= Mathf.Max(0, (float)(damage - PlayerProperty.Instance.defence));
        PlayerProperty.Instance.playerHealthBarController.UpdateHealthBarByITween(0.5f, PlayerProperty.Instance.oldHpForAnim, PlayerProperty.Instance.hp, PlayerProperty.Instance.maxHp);
        PlayerProperty.Instance.SetAndGetActiveBoneAnimator(PlayerState.Side, transform);
        PlayerProperty.Instance.isControlLocked = true;
        PlayerProperty.Instance.ChangeStatus(PlayerStatus.damaged);
        PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isDamaged);
        StartCoroutine(InvincibleCoroutine());
        StartCoroutine(KnockbackCoroutine(direction));
    }

    IEnumerator KnockbackCoroutine(float direction)
    {
        float timer = 0f;
        while (timer < PlayerProperty.Instance.knockbackTime)
        {
            timer += Time.deltaTime;
            transform.localScale = new Vector2(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            actionRigidbody2D.linearVelocity = new Vector2(direction * PlayerProperty.Instance.knockbackPower, actionRigidbody2D.linearVelocity.y);
            yield return null;
        }
    }

    IEnumerator InvincibleCoroutine()
    {
        PlayerProperty.Instance.isCanBeAttacked = false;
        float timer = 0f;
        float flashTime = PlayerProperty.Instance.flashInterval;
        SpriteRenderer spriteRenderer = PlayerProperty.Instance.playerSpriteRenderer;
        while (timer < PlayerProperty.Instance.invincibleTime)
        {
            timer += flashTime;
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashTime);
        }
        spriteRenderer.enabled = true;
        PlayerProperty.Instance.debugTextForCheckHit.gameObject.SetActive(false);
        PlayerProperty.Instance.isCanBeAttacked = true;
    }
}
