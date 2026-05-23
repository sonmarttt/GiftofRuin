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
    public float landDuration = 5f;
    public float minAirTime = 8f;
    public float maxAirTime = 14f;

    [Header("Projectile")]
    public GameObject spellProjectilePrefab;
    public Transform spellSpawnPoint;
    public float projectileSpeed = 8f;

    [Header("AOE")]
    public float aoeDamage = 20f;
    public float aoeRadius = 5f;

    // events for animator controller to listen to
    public static event System.Action OnSpellStart;
    public static event System.Action OnAOEStart;

    private Animator animator;
    private Transform player;
    private BossState currentState;

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
        transform.position = floatPosition;

        // listen for animation sync events
        BossAnimatorController.SpellFireEvent += FireProjectile;
        BossAnimatorController.AOEHitEvent += DealAOEDamage;

        SetState(BossState.Idle);
        StartCoroutine(BossRoutine());
    }

    void OnDestroy()
    {
        BossAnimatorController.SpellFireEvent -= FireProjectile;
        BossAnimatorController.AOEHitEvent -= DealAOEDamage;
    }

    void Update()
    {
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
        yield return new WaitForSeconds(3f);

        while (true)
        {
            SetState(BossState.Flying);
            float airTime = Random.Range(minAirTime, maxAirTime);
            float elapsed = 0f;

            while (elapsed < airTime)
            {
                elapsed += Time.deltaTime;

                Vector3 targetPos = new Vector3(player.position.x, floatPosition.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

                if (elapsed > 2f && Random.value < 0.005f)
                {
                    if (Random.Range(0, 2) == 0)
                        yield return StartCoroutine(DoAOEAttack());
                    else
                        yield return StartCoroutine(DoSpellProjectile());
                }

                yield return null;
            }

            yield return StartCoroutine(Land());
            yield return StartCoroutine(GoToFloat());
        }
    }

    IEnumerator DoAOEAttack()
    {
        SetState(BossState.AOEAttack);

        Vector3 hoverPos = new Vector3(transform.position.x, floatPosition.y, transform.position.z);
        transform.position = hoverPos;

        // fire event so animator controller starts timing
        OnAOEStart?.Invoke();

        // wait full animation length
        yield return new WaitForSeconds(2.167f);

        SetState(BossState.Flying);
    }

    IEnumerator DoSpellProjectile()
    {
        SetState(BossState.SpellProjectile);

        Vector3 hoverPos = new Vector3(transform.position.x, floatPosition.y, transform.position.z);
        transform.position = hoverPos;

        // fire event so animator controller starts timing
        OnSpellStart?.Invoke();

        // wait full animation length
        yield return new WaitForSeconds(2.300f);

        SetState(BossState.Flying);
    }

    void FireProjectile()
    {
        if (spellProjectilePrefab == null || spellSpawnPoint == null) return;

        GameObject proj = Instantiate(spellProjectilePrefab, spellSpawnPoint.position, Quaternion.identity);
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, player.position.z);
        Vector3 dir = (targetPos - spellSpawnPoint.position).normalized;

        BossProjectile bp = proj.GetComponent<BossProjectile>();
        if (bp != null) bp.SetDirection(dir);
    }

    void DealAOEDamage()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= aoeRadius)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.TakeDamage((int)aoeDamage);
        }
    }

    IEnumerator Land()
    {
        SetState(BossState.Landed);

        float t = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, 3.27f, transform.position.z);

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(landDuration);
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
                animator.SetBool("isFlying", true); // fly during idle too
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