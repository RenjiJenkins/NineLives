using UnityEngine;

public class BreakableBlock : MonoBehaviour, IShatterable
{
    public GameObject shatterEffectPrefab;

    public void Shatter()
    {
        // Optional: Spawn particle effect or play sound here
        if (shatterEffectPrefab != null)
        {
            Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"{gameObject.name} was shattered by the blade!");
        Destroy(gameObject);
    }
}