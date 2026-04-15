using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    public int hitsToKill = 3;
    private int currentHits = 0;

    public void TakeHit()
    {
        currentHits++;
        Debug.Log("NPC hit " + currentHits + "/" + hitsToKill);

        if (currentHits >= hitsToKill)
            Die();
    }

    private void Die()
    {
        Debug.Log("NPC died");
        Destroy(gameObject);
    }
}