using UnityEngine;

public class Player_Event_HurtedEnd : MonoBehaviour
{
    public void OnHurtedEnd()
    {
        PlayerProperty.Instance.isControlLocked = false;
        if(PlayerProperty.Instance.isGrounded)
            PlayerProperty.Instance.ChangeStatus(PlayerStatus.walk);
        else
            PlayerProperty.Instance.ChangeStatus(PlayerStatus.jump);
    }
}