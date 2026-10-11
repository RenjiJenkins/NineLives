using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    [SerializeField] private float speed = 25f;
    [SerializeField] private Color disablerColor = Color.cyan; // Color for ability-disabling bullet
    [SerializeField] private Color lethalColor = Color.red;    // Color for lethal bullet

    private bool disablesAbility;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(bool isAbilityDisabler, Vector3 targetPos)
    {
        disablesAbility = isAbilityDisabler;

        // Visual distinction between the two bullet types
        if (spriteRenderer != null)
        {
            spriteRenderer.color = disablesAbility ? disablerColor : lethalColor;
        }

        // Shoot straight toward the target position locked at creation time
        Vector2 direction = (targetPos - transform.position).normalized;
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // Rotate bullet sprite to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it hit the player
        if (other.CompareTag("Player"))
        {
            if (disablesAbility)
            {
                // SHOT 1: Disable the player's active modular ability components until death
                DisablePlayerAbilities(other.gameObject);
                Debug.Log("[Sniper Bullet] Player ability disabled!");
            }
            else
            {
                // SHOT 2: Standard lethal hit - triggers your LifeManager host-swap death loop
                TriggerPlayerDeath();
                Debug.Log("[Sniper Bullet] Lethal hit! Triggering death and host swap.");
            }

            Destroy(gameObject); // Destroy bullet on impact
        }
        else if (!other.CompareTag("Enemy"))
        {
            // Destroy on hitting walls/environment
            Destroy(gameObject);
        }
    }

    private void DisablePlayerAbilities(GameObject player)
    {
        // Disable modular ability scripts (Portal Gun, Grapple, etc.)
        PortalGunAbility portalAbility = player.GetComponent<PortalGunAbility>();
        if (portalAbility != null) portalAbility.enabled = false;

        GrappleAbility grappleAbility = player.GetComponent<GrappleAbility>();
        if (grappleAbility != null) grappleAbility.enabled = false;
    }

    private void TriggerPlayerDeath()
    {
        // Check if LifeManager exists in the scene and trigger the death/host-swap sequence
        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.TriggerDeathAndSwitchHost();
        }
        else
        {
            Debug.LogError("[Sniper Bullet] LifeManager instance not found in the scene!");
        }
    }
}