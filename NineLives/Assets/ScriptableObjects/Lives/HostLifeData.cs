using UnityEngine;

[CreateAssetMenu(fileName = "NewHostData", menuName = "NineLives/Host Data")]
public class HostLifeData : ScriptableObject
{
    public string hostName;
    public int lifeNumber; // 1 to 9
    
    [Header("Movement Stats")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public int maxJumps = 1;
    
    [Header("Visuals & Colliders")]
    public Sprite defaultSprite;
    public RuntimeAnimatorController animatorController;
    public Vector2 colliderSize = new Vector2(1f, 2f);
    public Vector2 colliderOffset = new Vector2(0f, 0f);
    
    [Header("Innate Ability")]
    public AbilityData innateAbility;
}