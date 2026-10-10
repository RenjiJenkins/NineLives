using UnityEngine;

public class PortalGunAbility : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private Transform handTransform; // The child object representing the hand
    [SerializeField] private GameObject portalPrefab;   // Portal prefab to spawn
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private LayerMask surfaceLayer;    // Surfaces portals can attach to

    [Header("Host Lock")]
    private int requiredLifeNumber = 4; 

    private PlayerController2D playerController;
    private GameObject activePortal;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        // LOCK CHECK: Only allow interaction if current host is the designated life
        if (playerController == null || playerController.currentHost == null) return;
        if (playerController.currentHost.lifeNumber != requiredLifeNumber) return;

        // Aim the hand towards the mouse cursor
        AimHandAtMouse();

        // Fire portal on Left Mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            ShootPortal();
        }
    }

    private void AimHandAtMouse()
    {
        if (handTransform == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Calculate direction and rotate the hand towards the mouse
        Vector2 direction = (mouseWorldPos - handTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        handTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void ShootPortal()
    {
        if (handTransform == null || portalPrefab == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 shootDirection = (mouseWorldPos - handTransform.position).normalized;

        // Raycast from hand to see if it hits a valid portal surface
        RaycastHit2D hit = Physics2D.Raycast(handTransform.position, shootDirection, 5f, surfaceLayer);

        if (hit.collider != null)
        {
            // If an active portal already exists, move it or destroy it
            if (activePortal != null)
            {
                Destroy(activePortal);
            }

            // Spawn portal at the hit point, aligned with the surface normal
            activePortal = Instantiate(portalPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Debug.Log("[Portal Ability] Portal created on surface!");
        }
        else
        {
            Debug.Log("[Portal Ability] Raycast missed a valid portal surface.");
        }
    }
}