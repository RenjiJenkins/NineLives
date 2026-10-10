using UnityEngine;

public class Hazard2D : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if player has the Barrier component active
            BarrierAbility barrier = other.GetComponent<BarrierAbility>();

            if (barrier != null && barrier.IsInvulnerable())
            {
                Debug.Log("[Hazard Ignored] Forcefield absorbed lethal damage!");
                return; // Player is protected! Do not trigger death.
            }

            // Otherwise, normal one-shot death happens
            Debug.Log("[Lethal Hit] Player took damage!");
            LifeManager.Instance.TriggerDeathAndSwitchHost();
        }
    }
}