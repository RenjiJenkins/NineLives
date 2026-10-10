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
        // Toggle transformation with 'C' key
        if (Input.GetKeyDown(KeyCode.C))
        {
            // LOCK CHECK: Only allow transformation if the active host is Life 1 (Julian)
            if (playerController != null && playerController.currentHost != null)
            {
                if (playerController.currentHost.lifeNumber == 0)
                {
                    ToggleCatForm();
                }
                else
                {
                    Debug.Log($"[Ability Locked] This host ({playerController.currentHost.hostName}) cannot transform into a cat! Only Life 1 (Julian) has this innate trait.");
                }
            }
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

        if (playerController != null && playerController.currentHost != null)
        {
            humanMoveSpeed = playerController.currentHost.moveSpeed;
            humanJumpForce = playerController.currentHost.jumpForce;

            playerController.currentHost.moveSpeed = catMoveSpeed;
            playerController.currentHost.jumpForce = catJumpForce;
        }

        boxCollider.size = catColliderSize;
        boxCollider.offset = catColliderOffset;

        if (animator != null && catAnimator != null)
        {
            animator.runtimeAnimatorController = catAnimator;
        }

        Debug.Log("[Ability] Transformed into Cat Form!");
    }

    private void TransformToHuman()
    {
        isCatForm = false;

        if (playerController != null && playerController.currentHost != null)
        {
            playerController.currentHost.moveSpeed = humanMoveSpeed;
            playerController.currentHost.jumpForce = humanJumpForce;
        }

        boxCollider.size = humanColliderSize;
        boxCollider.offset = humanColliderOffset;

        if (animator != null && humanAnimator != null)
        {
            animator.runtimeAnimatorController = humanAnimator;
        }

        Debug.Log("[Ability] Returned to Human Form!");
    }

    private bool CanStandUp()
    {
        if (headCheck == null) return true;
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