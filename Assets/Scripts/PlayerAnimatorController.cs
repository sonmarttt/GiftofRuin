using System.Collections;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;
    private Rigidbody rb;

    private int comboStep = 0;
    private float comboResetTime = 1.2f;
    private float comboTimer = 0f;
    private bool inputBuffered = false;
    private bool isAttacking = false;

    [Header("Sword Hitboxes")]
    public SwordHitbox rightSwordHitbox;
    public SwordHitbox leftSwordHitbox;

void Start()
{
    animator = GetComponent<Animator>();
    movement = GetComponent<PlayerMovement>();
    rb = GetComponent<Rigidbody>();

    // ADD THESE
    Debug.Log("Right hitbox: " + rightSwordHitbox);
    Debug.Log("Left hitbox: " + leftSwordHitbox);
}
    void Update()
    {
        animator.SetFloat("CharacterSpeed", rb.velocity.magnitude);
        animator.SetBool("IsGrounded", movement.IsGrounded);

        if (Input.GetButtonUp("Fire2"))
            animator.SetTrigger("doRoll");

        HandleCombo();
    }

    private void HandleCombo()
    {
        if (comboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                ResetCombo();
        }

if (Input.GetMouseButtonDown(0))
{
    Debug.Log("Click detected, comboStep: " + comboStep);
    if (comboStep == 0)
    {
        comboStep = 1;
        isAttacking = true;
        comboTimer = comboResetTime;
        animator.SetInteger("ComboStep", comboStep);
        Debug.Log("Starting coroutine for right hitbox");
        StartCoroutine(ActivateHitbox(rightSwordHitbox));
    }
    else if (comboStep < 3)
    {
        inputBuffered = true;
    }
}

        if (inputBuffered && comboStep > 0)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.7f)
            {
                inputBuffered = false;
                comboStep++;
                if (comboStep > 3) comboStep = 1;
                comboTimer = comboResetTime;
                animator.SetInteger("ComboStep", comboStep);

                if (comboStep == 2)
                    StartCoroutine(ActivateHitbox(leftSwordHitbox));
                else if (comboStep == 3)
                {
                    StartCoroutine(ActivateHitbox(rightSwordHitbox));
                    StartCoroutine(ActivateHitbox(leftSwordHitbox));
                }
            }
        }
    }

private IEnumerator ActivateHitbox(SwordHitbox hitbox)
{
    if (hitbox == null) yield break;
    hitbox.EnableHitbox(); // now calls DealDamageInRadius() internally
    yield return new WaitForSeconds(0.3f);
    hitbox.DisableHitbox();
}

    private void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        inputBuffered = false;
        comboTimer = 0f;
        animator.SetInteger("ComboStep", 0);
    }
}