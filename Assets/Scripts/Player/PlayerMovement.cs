using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAction))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    VariableJoystick variableJoystick;
    float minReaction = 0.05f;
    private Button jumpButton;
    private Button sprintButton;
    private Transform modelTransform;
    private SpriteRenderer shadowSpriteRenderer;
    private Image sprintButtonImage;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        variableJoystick = FindAnyObjectByType<VariableJoystick>();
        modelTransform = transform.Find("Devil");
        var shadowGameObject = Instantiate(Resources.Load<GameObject>("Prefab/PlayerShadow"), transform.position, transform.rotation);
        shadowGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        shadowSpriteRenderer = shadowGameObject.GetComponent<SpriteRenderer>();
        shadowGameObject.SetActive(false);
        var controllerUI = GameObject.FindGameObjectsWithTag(Tags.ControllerUI.ToString());
        foreach (var item in controllerUI)
        {
            if (item.name == "B_Jump")
                jumpButton = item.GetComponent<Button>();
            if (item.name == "B_Sprint")
                sprintButton = item.GetComponent<Button>();
        }
        jumpButton.onClick.AddListener(Jump);
        sprintButton.onClick.AddListener(Sprint);
        sprintButtonImage = sprintButton.GetComponent<Image>();
        sprintButtonImage.fillAmount = 1f;
    } 

    void Update()
    {
        if (GameplayPauseController.IsPaused)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        MoveInLine();
        if (Input.GetKeyDown(KeyCode.K)) Jump();
        if (Input.GetKeyDown(KeyCode.Z)) Sprint();
    }

    void MoveInLine()
    {
        float horizontal = variableJoystick.Horizontal;
        Vector2 direction = new Vector2(horizontal, 0);
        if (PlayerProperty.Instance.isControlLocked || PlayerProperty.Instance.isDashing) return;
        if (horizontal > minReaction)
        {
            PlayerProperty.Instance.direction = 1f;
            if (isTransToWalk())
            {
                PlayerProperty.Instance.SetAndGetActiveBoneAnimator(PlayerState.Side, modelTransform);
                PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
            }
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            rb.linearVelocity = direction * PlayerProperty.Instance.speed + new Vector2(0, rb.linearVelocity.y);
        }
        else if (horizontal < -minReaction)
        {
            PlayerProperty.Instance.direction = -1f;
            if (isTransToWalk())
            {
                PlayerProperty.Instance.SetAndGetActiveBoneAnimator(PlayerState.Side, modelTransform);
                PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
            }
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            rb.linearVelocity = direction * PlayerProperty.Instance.speed + new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            if (PlayerProperty.Instance.playerStatus == PlayerStatus.walk)
            {
                if (isTransToWalk())
                {
                    PlayerProperty.Instance.SetAndGetActiveBoneAnimator(PlayerState.Front, modelTransform);
                    PlayerProperty.Instance.ChangeStatus(PlayerStatus.idle);
                }
            }
        }
        if (PlayerProperty.Instance.playerStatus == PlayerStatus.walk)
        {
            PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isWalk);
        }
        else if (PlayerProperty.Instance.playerStatus == PlayerStatus.idle)
            PlayerProperty.Instance.ChangeCurrentAnimation(PlayerFrontAnimationTransParam.isIdle);
        else if (PlayerProperty.Instance.playerStatus == PlayerStatus.suprise && !PlayerProperty.Instance.isSupriseCovered)
            PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isWalk);
    }

    public bool isTransToWalk()
    {
        if (!PlayerProperty.Instance.isGrounded || PlayerProperty.Instance.isSupriseCovered)
        {
            return false;
        }
        return true;
    }

    void Jump()
    {
        if (!PlayerProperty.Instance.isGrounded || PlayerProperty.Instance.isControlLocked
        || PlayerProperty.Instance.isDashing) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, PlayerProperty.Instance.jumpPower);
    }

    void Sprint()
    {
        if (PlayerProperty.Instance.isControlLocked ||
        !PlayerProperty.Instance.canDash || PlayerProperty.Instance.isDashing) return;
        if (Mathf.Abs(variableJoystick.Horizontal) > minReaction)
            PlayerProperty.Instance.dashDirection = variableJoystick.Horizontal > 0 ? 1f : -1f;
        else
            PlayerProperty.Instance.dashDirection = transform.localScale.x > 0 ? -1f : 1f;
        StartCoroutine(DashCoroutine());
        StartCoroutine(PlayerProperty.Instance.CoolDownEffect(sprintButtonImage, PlayerProperty.Instance.dashCooldown));
    }

    IEnumerator DashCoroutine()
    {
        PlayerProperty.Instance.isDashing = true;
        PlayerProperty.Instance.canDash = false;
        PlayerProperty.Instance.SetAndGetActiveBoneAnimator(PlayerState.Side, modelTransform);
        PlayerProperty.Instance.ChangeStatus(PlayerStatus.sprint);
        PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isWalk);
        PlayerProperty.Instance.isCanBeAttacked = false;
        float timer = 0f;
        while (timer < PlayerProperty.Instance.dashDuration)
        {
            timer += Time.deltaTime;
            rb.linearVelocity = new Vector2(PlayerProperty.Instance.dashDirection * PlayerProperty.Instance.speed * PlayerProperty.Instance.dashSpeedMultiplier, rb.linearVelocity.y);
            yield return null;
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        PlayerProperty.Instance.isDashing = false;
        PlayerProperty.Instance.isCanBeAttacked = true;
        PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
        yield return new WaitForSeconds(PlayerProperty.Instance.dashCooldown);
        PlayerProperty.Instance.canDash = true;
    }
}
