using UnityEngine;

public class PortalGunAbility : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private Transform handTransform; 
    [SerializeField] private GameObject portalPrefab1; // e.g., Blue Portal Prefab
    [SerializeField] private GameObject portalPrefab2; // e.g., Orange Portal Prefab
    [SerializeField] private float maxShootDistance = 20f; 
    [SerializeField] private LayerMask surfaceLayer;    

    [Header("Host Lock")]
    [SerializeField] private int requiredLifeNumber = 4; 

    private PlayerController2D playerController;
    private GameObject activePortal1;
    private GameObject activePortal2;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (playerController == null || playerController.currentHost == null) return;
        if (playerController.currentHost.lifeNumber != requiredLifeNumber) return;

        AimHandAtMouse();

        // Left Click = Portal 1
        if (Input.GetMouseButtonDown(0))
        {
            ShootPortal(true);
        }
        // Right Click = Portal 2
        else if (Input.GetMouseButtonDown(1))
        {
            ShootPortal(false);
        }
    }

    private void AimHandAtMouse()
{
    if (handTransform == null || Camera.main == null) return;

    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector2 direction = (mouseWorldPos - handTransform.position).normalized;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Fixed from Rad2Down to Rad2Deg
    handTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
}

    private void ShootPortal(bool isPortalOne)
    {
        if (handTransform == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 shootDirection = (mouseWorldPos - handTransform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(handTransform.position, shootDirection, maxShootDistance, surfaceLayer);

        if (hit.collider != null)
        {
            GameObject prefabToSpawn = isPortalOne ? portalPrefab1 : portalPrefab2;

            if (isPortalOne)
            {
                if (activePortal1 != null) Destroy(activePortal1);
                activePortal1 = Instantiate(prefabToSpawn, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
            else
            {
                if (activePortal2 != null) Destroy(activePortal2);
                activePortal2 = Instantiate(prefabToSpawn, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }

            // Link portals together if both exist
            LinkPortals();

            Debug.Log($"[Portal Gun] Spawned {(isPortalOne ? "Portal 1" : "Portal 2")} on {hit.collider.name}");
        }
    }

    private void LinkPortals()
    {
        if (activePortal1 != null && activePortal2 != null)
        {
            Portal portal1Script = activePortal1.GetComponent<Portal>();
            Portal portal2Script = activePortal2.GetComponent<Portal>();

            if (portal1Script != null && portal2Script != null)
            {
                portal1Script.connectedPortal = activePortal2.transform;
                portal2Script.connectedPortal = activePortal1.transform;
            }
        }
    }
}