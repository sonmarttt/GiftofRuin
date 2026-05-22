using System.Collections;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;
    private Rigidbody rb;
    private WeaponManager weaponManager;

    private int comboStep = 0;
    private float comboResetTime = 1.2f;
    private float comboTimer = 0f;
    private bool inputBuffered = false;
    private bool isAttacking = false;

    [Header("Dual Sword Hitboxes")]
    public SwordHitbox rightSwordHitbox;
    public SwordHitbox leftSwordHitbox;

    [Header("Single Sword Hitboxes")]
    public SwordHitbox singleSwordHitbox;

    [Header("Two Handed Grip")]
    public Transform leftHandGrip; // empty GameObject positioned on sword where left hand should go

    void OnAnimatorIK(int layerIndex)
    {
        if (weaponManager == null || weaponManager.CurrentWeaponIndex != 1) return;
        if (!isAttacking) return;

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGrip.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandGrip.rotation);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        animator.SetFloat("CharacterSpeed", rb.velocity.magnitude);
        animator.SetBool("IsGrounded", movement.IsGrounded);

        if (weaponManager != null)
            animator.SetInteger("WeaponType", weaponManager.CurrentWeaponIndex);

        if (Input.GetButtonUp("Fire2"))
            animator.SetTrigger("doRoll");

        HandleCombo();

        // enable root motion only for single sword attacks
        // in Update()
        animator.applyRootMotion = (weaponManager != null && weaponManager.CurrentWeaponIndex == 1 && isAttacking);
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
            if (comboStep == 0)
            {
                comboStep = 1;
                isAttacking = true;
                movement.isAttacking = true;
                comboTimer = comboResetTime;
                animator.SetInteger("ComboStep", comboStep);
                StartCoroutine(ActivateHitboxForCurrentWeapon(1));
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
                StartCoroutine(ActivateHitboxForCurrentWeapon(comboStep));
            }
        }
    }

    private IEnumerator ActivateHitboxForCurrentWeapon(int step)
    {
        int weaponIndex = weaponManager != null ? weaponManager.CurrentWeaponIndex : 0;

        if (weaponIndex == 0) // dual swords
        {
            if (step == 1)
                yield return StartCoroutine(ActivateHitbox(rightSwordHitbox));
            else if (step == 2)
                yield return StartCoroutine(ActivateHitbox(leftSwordHitbox));
            else if (step == 3)
            {
                StartCoroutine(ActivateHitbox(rightSwordHitbox));
                yield return StartCoroutine(ActivateHitbox(leftSwordHitbox));
            }
        }
        else if (weaponIndex == 1) // single sword
        {
            yield return StartCoroutine(ActivateHitbox(singleSwordHitbox));
        }
    }

    private IEnumerator ActivateHitbox(SwordHitbox hitbox)
    {
        if (hitbox == null) yield break;
        hitbox.EnableHitbox();
        yield return new WaitForSeconds(0.3f);
        hitbox.DisableHitbox();
    }

    private void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        movement.isAttacking = false;
        inputBuffered = false;
        comboTimer = 0f;
        animator.SetInteger("ComboStep", 0);
    }
}