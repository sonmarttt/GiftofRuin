using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 15;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}