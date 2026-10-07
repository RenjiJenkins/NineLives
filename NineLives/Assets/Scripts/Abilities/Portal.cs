using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destinationPoint;
    public LayerMask teleportLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent infinite portal loops by checking layer/tag
        if (((1 << other.gameObject.layer) & teleportLayers) != 0 && destinationPoint != null)
        {
            other.transform.position = destinationPoint.position;
        }
    }
}