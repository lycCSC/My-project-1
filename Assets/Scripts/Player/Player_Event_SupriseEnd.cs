using NUnit.Framework;
using UnityEngine;

public class Player_Event_SupriseEnd : MonoBehaviour
{
    public void Anim_Event_EndSuprise()
    {
        PlayerProperty.Instance.isSupriseCovered = false;
        if (PlayerProperty.Instance.isGrounded)
            PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
        else
            PlayerProperty.Instance.ChangeStatus(PlayerStatus.jump);
    }
}
