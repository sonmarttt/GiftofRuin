using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private bool isAttacking = false;

    public void EnableHitbox() { isAttacking = true; }
    public void DisableHitbox() { isAttacking = false; }

    // Fires continuously while inside a collider
    private void OnTriggerStay(Collider other)
    {
        if (!isAttacking) return;

        NPCHealth npc = other.GetComponent<NPCHealth>();
        if (npc != null)
        {
            npc.TakeHit();
            Debug.Log("Hit NPC!");
            isAttacking = false; // Prevent hitting multiple times per swing
        }
    }
}