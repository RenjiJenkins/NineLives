using UnityEngine;

public class CatTransformationAbility : MonoBehaviour
{
    [Header("Form Status")]
    public bool isCatForm = false;

    [Header("Human Form Settings (Saved Defaults)")]
    private Vector2 humanColliderSize;
    private Vector2 humanColliderOffset;
    private float humanMoveSpeed;
    private float humanJumpForce;

    [Header("Cat Form Modifiers")]
    [SerializeField] private Vector2 catColliderSize = new Vector2(0.8f, 0.8f);
    [SerializeField] private Vector2 catColliderOffset = new Vector2(0f, -0.4f);
    [SerializeField] private float catMoveSpeed = 11f;
    [SerializeField] private float catJumpForce = 16f;
    [SerializeField] private RuntimeAnimatorController catAnimator;

    [Header("Ceiling / Un-transform Check")]
    [SerializeField] private Transform headCheck;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private float ceilingCheckRadius = 0.2f;

    private PlayerController2D playerController;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private RuntimeAnimatorController humanAnimator;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Cache initial human dimensions
        if (boxCollider != null)
        {
            humanColliderSize = boxCollider.size;
            humanColliderOffset = boxCollider.offset;
        }

        if (animator != null)
        {
            humanAnimator = animator.runtimeAnimatorController;
        }
    }

    private void Update()
    {
        // Toggle transformation with 'C' key (or custom input action)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCatForm();
        }
    }

    public void ToggleCatForm()
    {
        if (!isCatForm)
        {
            TransformToCat();
        }
        else
        {
            // Ensure there is room above before changing back to human form
            if (CanStandUp())
            {
                TransformToHuman();
            }
            else
            {
                Debug.Log("[Cat Form] Space too tight to transform back to human!");
            }
        }
    }

    private void TransformToCat()
    {
        isCatForm = true;

        // Cache human movement parameters before applying cat stats
        if (playerController != null && playerController.currentHost != null)
        {
            humanMoveSpeed = playerController.currentHost.moveSpeed;
            humanJumpForce = playerController.currentHost.jumpForce;

            // Apply boosted cat stats
            playerController.currentHost.moveSpeed = catMoveSpeed;
            playerController.currentHost.jumpForce = catJumpForce;
        }

        // Shrink collider for small tunnels
        boxCollider.size = catColliderSize;
        boxCollider.offset = catColliderOffset;

        // Swap animator controller if provided
        if (animator != null && catAnimator != null)
        {
            animator.runtimeAnimatorController = catAnimator;
        }

        Debug.Log("[Ability] Transformed into Cat Form!");
    }

    private void TransformToHuman()
    {
        isCatForm = false;

        // Restore human movement stats
        if (playerController != null && playerController.currentHost != null)
        {
            playerController.currentHost.moveSpeed = humanMoveSpeed;
            playerController.currentHost.jumpForce = humanJumpForce;
        }

        // Restore human collider
        boxCollider.size = humanColliderSize;
        boxCollider.offset = humanColliderOffset;

        // Restore human animator controller
        if (animator != null && humanAnimator != null)
        {
            animator.runtimeAnimatorController = humanAnimator;
        }

        Debug.Log("[Ability] Returned to Human Form!");
    }

    private bool CanStandUp()
    {
        if (headCheck == null) return true;
        // Check if obstacles exist overhead before returning to full human height
        return !Physics2D.OverlapCircle(headCheck.position, ceilingCheckRadius, ceilingLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (headCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headCheck.position, ceilingCheckRadius);
        }
    }
}