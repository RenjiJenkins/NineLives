using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarrierAbility : MonoBehaviour
{
    [Header("Barrier Settings")]
    [SerializeField] private float barrierDuration = 3f;
    [SerializeField] private float cooldownTime = 5f;
    
    [Header("Visual & Physics Components")]
    [SerializeField] private GameObject barrierVisualObject; // The blue circle sprite/shield
    [SerializeField] private CircleCollider2D barrierCollider; // Trigger collider protecting player

    [Header("Status Flags")]
    public bool isBarrierActive = false;
    private bool isCooldown = false;

    private PlayerController2D playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController2D>();

        // Ensure barrier defaults to turned OFF on start
        if (barrierVisualObject != null) barrierVisualObject.SetActive(false);
        if (barrierCollider != null) barrierCollider.enabled = false;
    }

    private void Update()
    {
        // Activate Barrier on 'E' key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryActivateBarrier();
        }
    }

    public void TryActivateBarrier()
    {
        // LOCK CHECK: Only allow activation if current active host is Life 2 (Marlow)
        if (playerController != null && playerController.currentHost != null)
        {
            if (playerController.currentHost.lifeNumber != 1)
            {
                Debug.Log($"[Ability Locked] {playerController.currentHost.hostName} cannot use Forcefield Barrier! Only Life 2 (Marlow) has this ability.");
                return;
            }
        }

        if (!isBarrierActive && !isCooldown)
        {
            StartCoroutine(ActivateBarrierRoutine());
        }
        else if (isCooldown)
        {
            Debug.Log("[Barrier] Ability is on cooldown!");
        }
    }

    private IEnumerator ActivateBarrierRoutine()
    {
        isBarrierActive = true;

        // Turn on the visual blue shield and its trigger collider
        if (barrierVisualObject != null) barrierVisualObject.SetActive(true);
        if (barrierCollider != null) barrierCollider.enabled = true;

        Debug.Log("[Barrier] Marlow's Forcefield activated!");

        // Active Shield Window
        yield return new WaitForSeconds(barrierDuration);

        // Turn off barrier
        if (barrierVisualObject != null) barrierVisualObject.SetActive(false);
        if (barrierCollider != null) barrierCollider.enabled = false;
        isBarrierActive = false;

        // Start Cooldown
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;

        Debug.Log("[Barrier] Forcefield is ready again!");
    }

    /// <summary>
    /// Helper check for Hazards / Death Triggers to read before killing the player.
    /// </summary>
    public bool IsInvulnerable()
    {
        return isBarrierActive;
    }
}