using UnityEngine;
using UnityEngine.AI;

public class NPCAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private PlayerMovement playerMovement;

    [Header("NPC Settings")]
    public Transform player;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    private float attackTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        Debug.Log("PlayerMovement found: " + (playerMovement != null));
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        ChasePlayer(distanceToPlayer);
        UpdateMovementAnimation();
        HandleAttack(distanceToPlayer);
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void UpdateMovementAnimation()
    {
        animator.SetFloat("CharacterSpeed", agent.velocity.magnitude);
    }

    private void HandleAttack(float distanceToPlayer)
    {
        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange && attackTimer <= 0f)
        {
            animator.SetTrigger("Attack");
            attackTimer = attackCooldown;

            if (playerMovement != null)
                playerMovement.TakeDamage(attackDamage);
            else
                Debug.Log("playerMovement is NULL");
        }
    }
}