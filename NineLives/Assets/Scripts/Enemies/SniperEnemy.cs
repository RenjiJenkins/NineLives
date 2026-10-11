using UnityEngine;
using System.Collections;

public class SniperEnemy : MonoBehaviour
{
    [Header("Sniper Settings")]
    [SerializeField] private Transform gunPivot;      // The part of the sniper that rotates to face the player
    [SerializeField] private Transform firePoint;     // Where bullets spawn
    [SerializeField] private GameObject bulletPrefab; // Standard bullet prefab
    [SerializeField] private float shootInterval = 1.0f; // Time between shots (1 second)

    [Header("Laser Sight (Optional Visuals)")]
    [SerializeField] private LineRenderer laserSight;

    private Transform playerTransform;
    private PlayerController2D playerController;
    private float shootTimer;
    private int shotCounter = 0; // Tracks alternating bullet types (0 = ability disable, 1 = lethal)

    private void Start()
    {
        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController2D>();
        }

        shootTimer = shootInterval;
        
        if (laserSight != null)
        {
            laserSight.positionCount = 2;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // 1. Constantly aim at the player
        AimAtPlayer();

        // 2. Handle shooting cooldown
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

        // Update laser sight visuals
        UpdateLaserSight();
    }

    private void AimAtPlayer()
    {
        if (gunPivot == null) return;

        Vector2 direction = (playerTransform.position - gunPivot.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Instantiate bullet
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        SniperBullet bulletScript = bulletObj.GetComponent<SniperBullet>();

        if (bulletScript != null)
        {
            // Determine bullet type based on the alternating counter
            // shotCounter 0 = Disables Ability, shotCounter 1 = Lethal Kill
            bool isAbilityDisabler = (shotCounter % 2 == 0);
            bulletScript.Initialize(isAbilityDisabler, playerTransform.position);

            Debug.Log($"[Sniper] Fired {(isAbilityDisabler ? "Ability-Disabling" : "Lethal")} bullet!");
        }

        // Alternate for the next shot
        shotCounter++;
    }

    private void UpdateLaserSight()
    {
        if (laserSight == null || firePoint == null || playerTransform == null) return;

        laserSight.SetPosition(0, firePoint.position);
        // Cast laser straight towards the player or forward from firePoint
        Vector2 dir = (playerTransform.position - firePoint.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, 30f);

        if (hit.collider != null)
        {
            laserSight.SetPosition(1, hit.point);
        }
        else
        {
            laserSight.SetPosition(1, firePoint.position + (Vector3)(dir * 30f));
        }
    }
}