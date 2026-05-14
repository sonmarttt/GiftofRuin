using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private bool isAttacking = false;

    public void EnableHitbox()
    {
        isAttacking = true;
        DealDamageInRadius();
    }

    public void DisableHitbox()
    {
        isAttacking = false;
    }

    public void DealDamageInRadius(float radius = 1.5f)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            NPCHealth npc = hit.GetComponentInParent<NPCHealth>();
            if (npc != null)
            {
                npc.TakeHit();
                Debug.Log("Hit NPC via overlap!");
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;
        NPCHealth npc = other.GetComponentInParent<NPCHealth>();
        if (npc != null)
        {
            npc.TakeHit();
            Debug.Log("Hit NPC via trigger!");
            isAttacking = false;
        }
    }
}