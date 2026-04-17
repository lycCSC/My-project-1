using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayerProperty : MonoBehaviour
{
    public float hp = 100;
    public float maxHp = 100f;
    public float oldHpForAnim = 100f;
    public int defence = 5;
    public int exp = 0;
    public int level = 1;
    public float speed = 2.0f;
    public float jumpPower = 10.0f;
    public bool isGrounded = true;
    public float direction = 1f;
    public bool invisible;
    public bool isSupriseCovered;
    public float attackSpeed;
    public float attackPower;
    public float knockbackPower = 2f;
    public Animator playerCurrentAnimator;
    public PlayerState playerState;
    public PlayerStatus playerStatus;
    public PlayerStatus previousPlayerStatus;
    public string modelName = "Devil";
    private Dictionary<PlayerSideAnimationTransParam, int> sideParamterHash = new();
    private Dictionary<PlayerFrontAnimationTransParam, int> frontParamterHash = new();
    private Dictionary<PlayerFloatAnimationTransParam, int> floatParamterHash = new();
    public float invincibleTime = 1f;
    public float flashInterval = 0.1f;
    public bool isCanBeAttacked = true;
    public bool isControlLocked = false;
    public SpriteRenderer playerSpriteRenderer;
    public float knockbackTime = 0.3f;
    public float dashSpeedMultiplier = 3f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    public bool isDashing = false;
    public bool canDash = true;
    public float dashDirection = 1f;
    public float shadowFadeDuration = 1f;
    public float scaleChangeRatio = 0.003f;
    public PlayerHealthBarController playerHealthBarController;



    public TextMeshProUGUI debugTextForCheckHit;
    public TextMeshProUGUI debugTextForCheckDash;


    void Start()
    {
        hp = maxHp;
        ApplicationController.Instance.gameObject.SetActive(true);
        playerState = PlayerState.Front;
        playerStatus = PlayerStatus.idle;
        playerHealthBarController = GameObject.FindAnyObjectByType<PlayerHealthBarController>();
        InitAnimator();
        InitDebugText();
    }

    public void InitDebugText()
    {
        var debugTexts = GameObject.FindGameObjectsWithTag(Tags.DebugUI.ToString());
        foreach (var item in debugTexts)
        {
            if (item.name == "T_Debug_CheckHit")
                debugTextForCheckHit = item.GetComponent<TextMeshProUGUI>();
            else if (item.name == "T_Debug_CheckDash")
                debugTextForCheckDash = item.GetComponent<TextMeshProUGUI>();
        }
        Debug.Log(debugTextForCheckHit);
        Debug.Log(debugTextForCheckDash);
        debugTextForCheckDash.gameObject.SetActive(false);
        debugTextForCheckHit.gameObject.SetActive(false);
    }

    public IEnumerator CoolDownEffect(Image cooldownImage, float cd)
    {
        float t = 0f;
        cooldownImage.fillAmount = 0f;
        while (t < cd)
        {
            t += Time.deltaTime;
            cooldownImage.fillAmount = t / cd;
            yield return null;
        }
        cooldownImage.fillAmount = 1f;
    }

    public void ChangeStatus(PlayerStatus status)
    {
        previousPlayerStatus = playerStatus;
        playerStatus = status;
    }

    public void ChangeCurrentAnimation(PlayerSideAnimationTransParam animTransParam)
    {
        foreach (var itemParameter in sideParamterHash)
        {
            playerCurrentAnimator.SetBool(itemParameter.Value, itemParameter.Key == animTransParam);
        }
    }

    public void ChangeCurrentAnimation(PlayerFrontAnimationTransParam animTransParam)
    {
        foreach (var itemParameter in frontParamterHash)
        {
            playerCurrentAnimator.SetBool(itemParameter.Value, itemParameter.Key == animTransParam);
        }
    }

    public void ChangeCurrentAnimation(PlayerFloatAnimationTransParam animTransParam, float value)
    {
        foreach (var itemParameter in floatParamterHash)
        {
            if (itemParameter.Value == floatParamterHash[animTransParam])
            {
                playerCurrentAnimator.SetFloat(itemParameter.Value, value);
            }
        }
    }

    public void SetAndGetActiveBoneAnimator(PlayerState stateName, Transform selfTransform)
    {
        if (stateName == playerState) return;
        foreach (var item in PlayerState.GetValues(typeof(PlayerState)))
        {
            if (item.ToString() == stateName.ToString())
            {
                Transform targetTransform = FindChildInAnyLayer.FindChildRecursive(selfTransform, item.ToString());
                targetTransform.gameObject.SetActive(true);
                playerState = (PlayerState)item;
                playerCurrentAnimator = targetTransform.GetComponent<Animator>();
                playerSpriteRenderer = targetTransform.GetComponent<SpriteRenderer>();
                if (!isGrounded)
                {
                    if (PlayerProperty.Instance.playerState == PlayerState.Side)
                        ChangeCurrentAnimation(PlayerSideAnimationTransParam.isJump);
                    if (PlayerProperty.Instance.playerState == PlayerState.Front)
                    {
                        ChangeCurrentAnimation(PlayerFrontAnimationTransParam.isJump);
                    }
                }
                if (isSupriseCovered)
                {
                    ChangeCurrentAnimation(PlayerSideAnimationTransParam.isSuprise);
                    ChangeCurrentAnimation(PlayerFloatAnimationTransParam.attackSpeed, attackSpeed);
                }
            }
            else
                FindChildInAnyLayer.FindChildRecursive(selfTransform, item.ToString()).gameObject.SetActive(false);
        }
    }

    void InitAnimator()
    {
        foreach (PlayerSideAnimationTransParam item in System.Enum.GetValues(typeof(PlayerSideAnimationTransParam)))
        {
            sideParamterHash[item] = Animator.StringToHash(item.ToString());
        }
        foreach (PlayerFrontAnimationTransParam item in System.Enum.GetValues(typeof(PlayerFrontAnimationTransParam)))
        {
            frontParamterHash[item] = Animator.StringToHash(item.ToString());
        }
        foreach (PlayerFloatAnimationTransParam item in System.Enum.GetValues(typeof(PlayerFloatAnimationTransParam)))
        {
            floatParamterHash[item] = Animator.StringToHash(item.ToString());
        }
        Transform playerTF = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).transform;
        Transform modelTransform = playerTF.Find(modelName);
        for (int i = 0; i < modelTransform.childCount; i++)
        {
            Transform playerStateTransform = modelTransform.GetChild(i);
            if (playerStateTransform.name == playerState.ToString())
            {
                playerStateTransform.gameObject.SetActive(true);
                playerCurrentAnimator = playerStateTransform.GetComponent<Animator>();
            }
            else
            {
                playerStateTransform.gameObject.SetActive(false);
            }
        }
    }

    static PlayerProperty instance;

    static public PlayerProperty Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindAnyObjectByType<PlayerProperty>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("PlayerProperty");
                    instance = obj.AddComponent<PlayerProperty>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    void Awake()
    {
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
}
