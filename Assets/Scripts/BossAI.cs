using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Settings")]
    public float chaseSpeed = 5f;
    public float landDuration = 5f;
    public float hoverHeight = 5f;
    public float hoverDistance = 8f;

    [Header("Projectile")]
    public GameObject spellProjectilePrefab;
    public Transform spellSpawnPoint;

    [Header("AOE")]
    public float aoeDamage = 20f;
    public float aoeRadius = 5f;
    public GameObject aoePrefab;

    public static event System.Action OnSpellStart;
    public static event System.Action OnAOEStart;

    private Animator animator;
    private Transform player;
    private BossState currentState;
    private bool isLanded = false;

    private enum BossState { Idle, Flying, AOEAttack, SpellProjectile, Landed }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);

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
            isLanded = false;
            SetState(BossState.Flying);

            // do 1-2 attacks then land
            int attackCount = Random.Range(1, 3);
            for (int i = 0; i < attackCount; i++)
            {
                // fly around before each attack
                float flyTime = Random.Range(3f, 6f);
                float elapsed = 0f;

                while (elapsed < flyTime)
                {
                    elapsed += Time.deltaTime;

                    Vector3 dirToPlayer = (transform.position - player.position).normalized;
                    dirToPlayer.y = 0;
                    if (dirToPlayer == Vector3.zero) dirToPlayer = Vector3.forward;
                    Vector3 targetPos = player.position + dirToPlayer * hoverDistance;
                    targetPos.y = hoverHeight;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

                    yield return null;
                }

                // execute attack
                if (Random.Range(0, 2) == 0)
                    yield return StartCoroutine(DoAOEAttack());
                else
                    yield return StartCoroutine(DoSpellProjectile());

                SetState(BossState.Flying);
            }

            // land after attacks done
            yield return StartCoroutine(Land());
            yield return StartCoroutine(GoToFloat());
        }
    }

    IEnumerator DoAOEAttack()
    {
        SetState(BossState.AOEAttack);
        transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
        OnAOEStart?.Invoke();
        yield return new WaitForSeconds(2.167f);
    }

    IEnumerator DoSpellProjectile()
    {
        SetState(BossState.SpellProjectile);
        transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
        OnSpellStart?.Invoke();
        yield return new WaitForSeconds(2.300f);
    }

    void FireProjectile()
    {
        if (spellProjectilePrefab == null || spellSpawnPoint == null) return;
        GameObject proj = Instantiate(spellProjectilePrefab, spellSpawnPoint.position, Quaternion.identity);
        Vector3 dir = (player.position - spellSpawnPoint.position).normalized;
        BossProjectile bp = proj.GetComponent<BossProjectile>();
        if (bp != null) bp.SetDirection(dir);
    }

    void DealAOEDamage()
    {
        if (aoePrefab != null)
            Instantiate(aoePrefab, new Vector3(player.position.x, player.position.y, player.position.z), Quaternion.identity);
    }

    IEnumerator Land()
    {
        Debug.Log("Boss landing");
        isLanded = true;
        SetState(BossState.Landed);

        float t = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, -1f, transform.position.z);

        Debug.Log("Landing from Y: " + startPos.y + " to Y: " + endPos.y);

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(landDuration);
        isLanded = false;
    }

    IEnumerator GoToFloat()
    {
        SetState(BossState.Flying);

        float t = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, hoverHeight, transform.position.z);

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    void SetState(BossState newState)
    {
        currentState = newState;
        animator.SetBool("isFlying", false);
        animator.SetBool("isLanded", false);

        switch (newState)
        {
            case BossState.Idle:
            case BossState.Flying:
                animator.SetBool("isFlying", true);
                break;
            case BossState.AOEAttack:
                animator.SetTrigger("isAOE");
                break;
            case BossState.SpellProjectile:
                animator.SetTrigger("isSpell");
                break;
            case BossState.Landed:
                animator.SetBool("isLanded", true);
                break;
        }
    }

    public bool IsLanded() => isLanded;
}