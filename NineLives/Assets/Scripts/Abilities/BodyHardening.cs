using System.Collections;
using UnityEngine;

public class BodyHardening : MonoBehaviour
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private float hardenedMass = 10f;
    
    private Rigidbody2D rb;
    private float originalMass;
    private bool isHardened;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void ToggleHardening()
    {
        if (!isHardened) StartCoroutine(HardenRoutine());
    }

    private IEnumerator HardenRoutine()
    {
        isHardened = true;
        originalMass = rb.mass;
        rb.mass = hardenedMass;
        
        // Visual indicator logic (e.g., stone shader or gray color)
        GetComponent<SpriteRenderer>().color = Color.gray;

        yield return new WaitForSeconds(duration);

        rb.mass = originalMass;
        GetComponent<SpriteRenderer>().color = Color.white;
        isHardened = false;
    }
}