using UnityEngine;

public class GrappleAbility : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private Transform firePoint; // Where the hook shoots from (e.g., hand or chest)
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private LayerMask grappleableLayer;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Rope Physics")]
    [SerializeField] private float reelSpeed = 5f; // Speed to pull closer or push out

    [Header("Host Lock")]
    [SerializeField] private int requiredLifeNumber = 5; // Life index for this ability

    private PlayerController2D playerController;
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
        rb = GetComponent<Rigidbody2D>();
        
        springJoint = GetComponent<SpringJoint2D>();
        if (springJoint == null)
        {
            springJoint = gameObject.AddComponent<SpringJoint2D>();
        }
        springJoint.enabled = false;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
        }
    }

    private void Update()
    {
        // Check for 'Q' key press
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("[Grapple] Q Key pressed! Checking grapple state...");

            if (!isGrappling)
            {
                TryShootGrapple();
            }
            else
            {
                StopGrapple();
            }
        }

        // Host lock checks
        if (playerController == null || playerController.currentHost == null) return;
        if (playerController.currentHost.lifeNumber != requiredLifeNumber) return;

        // If currently grappling, allow reeling in/out with W and S keys
        if (isGrappling)
        {
            HandleReeling();
            UpdateRopeVisuals();
        }
    }

    private void TryShootGrapple()
    {
        if (Camera.main == null)
        {
            Debug.LogError("[Grapple] Camera.main is missing!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("[Grapple] FirePoint transform is not assigned in the Inspector!");
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        // Draw a debug ray in the Scene view to see where it's shooting
        Debug.DrawRay(firePoint.position, direction * maxDistance, Color.magenta, 2f);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, maxDistance, grappleableLayer);

        if (hit.collider != null)
        {
            isGrappling = true;
            grapplePoint = hit.point;

            springJoint.enabled = true;
            springJoint.connectedAnchor = grapplePoint;
            
            float currentDistance = Vector2.Distance(transform.position, grapplePoint);
            springJoint.distance = Mathf.Clamp(currentDistance, 1f, maxDistance);

            springJoint.autoConfigureDistance = false;
            springJoint.dampingRatio = 0.5f;
            springJoint.frequency = 2f;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }

            Debug.Log($"[Grapple] SUCCESS! Hook latched onto {hit.collider.name} at position {hit.point}");
        }
        else
        {
            Debug.LogWarning("[Grapple] Raycast missed! Check if your ceiling/walls have a Collider2D and are assigned to the GrappleSurface layer.");
        }
    }

    private void HandleReeling()
    {
        if (Input.GetKey(KeyCode.W))
        {
            springJoint.distance = Mathf.Max(springJoint.distance - (reelSpeed * Time.deltaTime), 2f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            springJoint.distance = Mathf.Min(springJoint.distance + (reelSpeed * Time.deltaTime), maxDistance);
        }
    }

    private void UpdateRopeVisuals()
    {
        if (lineRenderer != null && isGrappling)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        springJoint.enabled = false;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        Debug.Log("[Grapple] Released hook.");
    }
}