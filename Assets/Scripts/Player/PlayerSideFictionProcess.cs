using UnityEngine;

public class PlayerSideFictionProcess : MonoBehaviour
{
    PhysicsMaterial2D physicsMaterial2D;

    void Start()
    {
        physicsMaterial2D = GetComponent<BoxCollider2D>().sharedMaterial;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            physicsMaterial2D.friction = 0f;
        }
    }
}
