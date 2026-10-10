using UnityEngine;

public class SuperStrengthAbility : MonoBehaviour
{
    [Header("Super Strength Settings")]
    [SerializeField] private Transform holdPoint; // Position directly above player's head
    [SerializeField] private float grabRange = 2f; 
    [SerializeField] private LayerMask grabbableLayer;

    [Header("Throw Settings")]
    [SerializeField] private float throwSpeed = 15f; // Overall speed of the throw towards the mouse

    [Header("Host Lock")]
    private int requiredLifeNumber = 2; // Life 3

    private GameObject heldObject;
    private Rigidbody2D heldObjectRb;
    private BoxCollider2D heldObjectCollider;
    private PlayerController2D playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        // LOCK CHECK: Only allow interaction if current host is Life 3
        if (playerController == null || playerController.currentHost == null) return;
        if (playerController.currentHost.lifeNumber != requiredLifeNumber) return;

        // Press 'F' to Lift or Throw
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
            {
                TryLiftObject();
            }
            else
            {
                ThrowObjectAtMouse();
            }
        }
    }

    private void TryLiftObject()
    {
        Collider2D targetHit = Physics2D.OverlapCircle(transform.position, grabRange, grabbableLayer);

        if (targetHit != null && targetHit.CompareTag("Grabbable"))
        {
            heldObject = targetHit.gameObject;
            heldObjectRb = heldObject.GetComponent<Rigidbody2D>();
            heldObjectCollider = heldObject.GetComponent<BoxCollider2D>();

            if (heldObjectRb != null)
            {
                heldObjectRb.simulated = false;
            }

            if (heldObjectCollider != null)
            {
                heldObjectCollider.enabled = false;
            }

            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;

            Debug.Log($"[Super Strength] Successfully lifted {heldObject.name} overhead!");
        }
        else
        {
            Debug.LogWarning("[Super Strength] No valid grabbable object matched the LayerMask & Tag criteria!");
        }
    }

    private void ThrowObjectAtMouse()
    {
        if (heldObject == null) return;

        if (heldObjectCollider != null)
        {
            heldObjectCollider.enabled = true;
        }

        heldObject.transform.SetParent(null);

        if (heldObjectRb != null)
        {
            heldObjectRb.simulated = true;

            // 1. Get mouse position in world space
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f; // Keep it on the 2D plane

            // 2. Calculate direction vector from player to mouse
            Vector2 throwDirection = (mouseWorldPos - transform.position).normalized;

            // 3. Apply velocity towards the mouse cursor
            heldObjectRb.linearVelocity = throwDirection * throwSpeed;
        }

        Debug.Log("[Super Strength] Thrown object toward mouse direction!");
        heldObject = null;
        heldObjectRb = null;
        heldObjectCollider = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}