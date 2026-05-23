using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 25;
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
            // check NPC
            NPCHealth npc = hit.GetComponentInParent<NPCHealth>();
            if (npc != null)
            {
                npc.TakeHit();
                Debug.Log("Hit NPC via overlap!");
                return;
            }

            // check boss
            BossHealth boss = hit.GetComponentInParent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("Hit Boss via overlap!");
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // check NPC
        NPCHealth npc = other.GetComponentInParent<NPCHealth>();
        if (npc != null)
        {
            npc.TakeHit();
            Debug.Log("Hit NPC via trigger!");
            isAttacking = false;
            return;
        }

        // check boss
        BossHealth boss = other.GetComponentInParent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Debug.Log("Hit Boss via trigger!");
            isAttacking = false;
        }
    }
}