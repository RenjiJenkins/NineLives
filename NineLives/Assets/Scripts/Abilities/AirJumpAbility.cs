using UnityEngine;

[CreateAssetMenu(fileName = "AirJump", menuName = "NineLives/Abilities/AirJump")]
public class AirJumpAbility : AbilityData
{
    public float extraJumpForce = 12f;

    public override void Activate(GameObject user)
    {
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, extraJumpForce);
        }
    }
}