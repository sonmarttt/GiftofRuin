using UnityEngine;

public class GrabbableCorpse : MonoBehaviour
{
    public bool IsDead { get; private set; } = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true; // starts kinematic, NPC is controlled by NavMesh
    }

    public void Die()
    {
        IsDead = true;
        // don't touch the rigidbody at all
        // animation handles falling to the floor
    }
public void Grab()
{
    rb.isKinematic = true;
    GetComponent<Collider>().enabled = false; // stop it pushing player
}

public void MoveToPosition(Vector3 targetPos)
{
    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 15f);
}

public void Release()
{
    foreach (var col in GetComponentsInChildren<Collider>())
        col.enabled = false;

    if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 20f))
        transform.position = new Vector3(transform.position.x, hit.point.y - 1.0f, transform.position.z);

    foreach (var col in GetComponentsInChildren<Collider>())
        col.enabled = true;
}
}