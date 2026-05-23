using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 15;
    public float lifetime = 5f;
    public float speed = 12f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

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