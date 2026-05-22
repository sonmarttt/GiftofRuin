using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Positions")]
    private Vector3 floatPosition;
    private Vector3 landPosition;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float attackRange = 5f;
    public float landDuration = 5f;       // how long he stays on ground
    public float minAirTime = 8f;         // min time before landing again
    public float maxAirTime = 14f;        // max time before landing again

    [Header("Projectile")]
    public GameObject spellProjectilePrefab;
    public Transform spellSpawnPoint;
    public float projectileSpeed = 8f;

    [Header("AOE")]
    public float aoeDamage = 20f;
    public float aoeRadius = 5f;

    private Animator animator;
    private Transform player;
    private BossState currentState;
    private float stateTimer = 0f;
    private bool isLanding = false;

    private enum BossState
    {
        Idle,
        Flying,
        AOEAttack,
        SpellProjectile,
        Landed
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        floatPosition = new Vector3(1.002f, 8.63f, 0.04f);
        landPosition = new Vector3(transform.position.x, 3.27f, transform.position.z);

        transform.position = floatPosition;

        SetState(BossState.Idle);
        StartCoroutine(BossRoutine());
    }

    void Update()
    {
        // always face player
        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(dir), 5f * Time.deltaTime);
        }
    }

    IEnumerator BossRoutine()
    {
        // start with idle for 3 seconds
        yield return new WaitForSeconds(3f);

        while (true)
        {
            // fly around and chase player
            SetState(BossState.Flying);
            float airTime = Random.Range(minAirTime, maxAirTime);
            float elapsed = 0f;

            while (elapsed < airTime)
            {
                elapsed += Time.deltaTime;

                // move toward player while flying
                Vector3 targetPos = new Vector3(player.position.x, floatPosition.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

                // randomly do air attacks
                if (elapsed > 2f && Random.value < 0.005f)
                {
                    int attackChoice = Random.Range(0, 2);
                    if (attackChoice == 0)
                    {
                        yield return StartCoroutine(DoAOEAttack());
                    }
                    else
                    {
                        yield return StartCoroutine(DoSpellProjectile());
                    }
                }

                yield return null;
            }

            // land for player to attack
            yield return StartCoroutine(Land());

            // go back up
            yield return StartCoroutine(GoToFloat());
        }
    }

    IEnumerator DoAOEAttack()
    {
        SetState(BossState.AOEAttack);

        // hover in place during spell
        Vector3 hoverPos = new Vector3(transform.position.x, floatPosition.y, transform.position.z);
        transform.position = hoverPos;

        yield return new WaitForSeconds(2f); // animation plays

        // deal AOE damage
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= aoeRadius)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.TakeDamage((int)aoeDamage);
        }

        yield return new WaitForSeconds(1f);
        SetState(BossState.Flying);
    }

    IEnumerator DoSpellProjectile()
    {
        SetState(BossState.SpellProjectile);

        // hover in place during spell
        Vector3 hoverPos = new Vector3(transform.position.x, floatPosition.y, transform.position.z);
        transform.position = hoverPos;

        yield return new WaitForSeconds(1f); // wind up

        if (spellProjectilePrefab != null && spellSpawnPoint != null)
        {
            GameObject proj = Instantiate(spellProjectilePrefab, spellSpawnPoint.position, Quaternion.identity);
            Vector3 dir = (player.position - spellSpawnPoint.position).normalized;
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = dir * projectileSpeed;
        }

        yield return new WaitForSeconds(1f);
        SetState(BossState.Flying);
    }

    IEnumerator Land()
    {
        isLanding = true;
        SetState(BossState.Landed);

        // smoothly move down
        float t = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, 3.27f, transform.position.z);

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // stay on ground for landDuration
        yield return new WaitForSeconds(landDuration);
        isLanding = false;
    }

    IEnumerator GoToFloat()
    {
        SetState(BossState.Flying);

        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, floatPosition, t);
            yield return null;
        }
    }

    void SetState(BossState newState)
    {
        currentState = newState;

        animator.SetBool("isFlying", false);
        animator.SetBool("isAOE", false);
        animator.SetBool("isSpell", false);
        animator.SetBool("isLanded", false);

        switch (newState)
        {
            case BossState.Idle:
                break;
            case BossState.Flying:
                animator.SetBool("isFlying", true);
                break;
            case BossState.AOEAttack:
                animator.SetBool("isAOE", true);
                break;
            case BossState.SpellProjectile:
                animator.SetBool("isSpell", true);
                break;
            case BossState.Landed:
                animator.SetBool("isLanded", true);
                break;
        }
    }

    public bool IsLanded() => currentState == BossState.Landed;
}