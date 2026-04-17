using UnityEngine;

public class PlayerJumpProcess : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (PlayerProperty.Instance.playerState == PlayerState.Front)
                PlayerProperty.Instance.ChangeStatus(PlayerStatus.idle);
            else if (PlayerProperty.Instance.playerState == PlayerState.Side)
            {
                if (PlayerProperty.Instance.playerStatus != PlayerStatus.suprise)
                    PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
            }
            PlayerProperty.Instance.isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PlayerProperty.Instance.isGrounded = false;
            PlayerProperty.Instance.ChangeStatus(PlayerStatus.jump);
            if (PlayerProperty.Instance.playerState == PlayerState.Side)
                PlayerProperty.Instance.ChangeCurrentAnimation(PlayerSideAnimationTransParam.isJump);
            else if (PlayerProperty.Instance.playerState == PlayerState.Front)
                PlayerProperty.Instance.ChangeCurrentAnimation(PlayerFrontAnimationTransParam.isJump);
        }
    }
}
