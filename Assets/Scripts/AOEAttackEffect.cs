using System.Collections;
using UnityEngine;

public class AOEAttackEffect : MonoBehaviour
{
    public float radius = 4f;
    public int damage = 25;
    public float warningDuration = 0.8f;
    public float strikeDuration = 0.4f;

    private ParticleSystem strikeEffect;
    private bool hasDealtDamage = false;

    void Start()
    {
        // find it automatically from children
        strikeEffect = GetComponentInChildren<ParticleSystem>();

        if (strikeEffect != null)
        {
            Debug.Log("Strike effect found: " + strikeEffect.name);
            strikeEffect.Stop();
        }
        else
        {
            Debug.Log("NO PARTICLE SYSTEM FOUND IN CHILDREN");
        }

        StartCoroutine(AOESequence());
    }

    IEnumerator AOESequence()
    {
        Debug.Log("AOE Warning phase started");
        yield return new WaitForSeconds(warningDuration);

        Debug.Log("AOE Strike phase started");
        if (strikeEffect != null)
        {
            Debug.Log("Playing strike effect");
            strikeEffect.Play();
        }

        if (!hasDealtDamage)
        {
            hasDealtDamage = true;
            DealDamage();
        }

        yield return new WaitForSeconds(strikeDuration);
        Destroy(gameObject);
    }

    void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerMovement pm = hit.GetComponent<PlayerMovement>();
                if (pm != null) pm.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}