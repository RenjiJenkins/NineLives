using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [HideInInspector] public Transform connectedPortal; 
    private static bool isTeleporting = false; // Prevents infinite loops between portals

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return; // Exit if a teleport is already processing

        if (other.CompareTag("Player") && connectedPortal != null)
        {
            StartCoroutine(TeleportRoutine(other.transform));
        }
    }

    private IEnumerator TeleportRoutine(Transform playerTransform)
    {
        isTeleporting = true;

        // Calculate exit position slightly outward from the connected portal using its up vector
        Vector3 exitPosition = connectedPortal.position + (connectedPortal.up * 0.8f);
        playerTransform.position = exitPosition;

        Debug.Log("[Portal] Teleported player safely to connected portal.");

        // Wait a fraction of a second so the player clears the trigger box before re-enabling teleports
        yield return new WaitForSeconds(0.2f);

        isTeleporting = false;
    }
}