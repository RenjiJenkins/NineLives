using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LifeManager.Instance.SetNewCheckpoint(transform.position);
            Debug.Log($"[Checkpoint] Spawn point updated to {transform.position}");
        }
    }
}