using UnityEngine;

public class SwordCutAbility : MonoBehaviour
{
    [Header("Sword Settings")]
    [SerializeField] private Transform attackPoint; // Point in front of the player
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private LayerMask shatterableLayer;

    [Header("Host Lock")]
    [SerializeField] private int requiredLifeNumber = 3; // Life 4

    private PlayerController2D playerController;
    private Animator animator;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // LOCK CHECK: Only allow the sword cut if current host is Life 4
        if (playerController == null || playerController.currentHost == null) return;
        if (playerController.currentHost.lifeNumber != requiredLifeNumber) return;

        // Press 'R' or Left Click to Swing Sword
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
        {
            PerformSwordCut();
        }
    }

    private void PerformSwordCut()
    {
        // Trigger slash animation if available
        if (animator != null)
        {
            animator.SetTrigger("Slash");
        }

        // Detect objects in front of the attack point
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, shatterableLayer);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Shatterable"))
            {
                // Check if the object has a breakable component or destroy it directly
                IShatterable shatterable = hit.GetComponent<IShatterable>();
                if (shatterable != null)
                {
                    shatterable.Shatter();
                }
                else
                {
                    Destroy(hit.gameObject);
                }

                Debug.Log($"[Sword Cut] Shattered obstacle: {hit.gameObject.name}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}