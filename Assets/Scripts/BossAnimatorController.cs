using System.Collections;
using UnityEngine;

public class BossAnimatorController : MonoBehaviour
{
    // Animation lengths
    private const float AOE_LENGTH = 2.167f;
    private const float SPELL_LENGTH = 2.300f;

    // Fire projectile at 40% through the spell animation
   private const float SPELL_FIRE_POINT = 0.345f;

    // AOE damage lands at 60% through the AOE animation
    private const float AOE_HIT_POINT = 0.6f;

    private Animator animator;
    private BossAI bossAI;

    public delegate void OnSpellFire();
    public delegate void OnAOEHit();

    public static event OnSpellFire SpellFireEvent;
    public static event OnAOEHit AOEHitEvent;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossAI = GetComponent<BossAI>();

        BossAI.OnSpellStart += HandleSpellStart;
        BossAI.OnAOEStart += HandleAOEStart;
    }

    void OnDestroy()
    {
        BossAI.OnSpellStart -= HandleSpellStart;
        BossAI.OnAOEStart -= HandleAOEStart;
    }

    void HandleSpellStart()
    {
        StartCoroutine(SpellSequence());
    }

    void HandleAOEStart()
    {
        StartCoroutine(AOESequence());
    }

    IEnumerator SpellSequence()
    {
        // wait until the animation reaches the throw point
        yield return new WaitForSeconds(SPELL_LENGTH * SPELL_FIRE_POINT);
        SpellFireEvent?.Invoke();

        // wait for rest of animation
        yield return new WaitForSeconds(SPELL_LENGTH * (1f - SPELL_FIRE_POINT));
    }

    IEnumerator AOESequence()
    {
        // wait until the animation reaches the impact point
        yield return new WaitForSeconds(AOE_LENGTH * AOE_HIT_POINT);
        AOEHitEvent?.Invoke();

        // wait for rest of animation
        yield return new WaitForSeconds(AOE_LENGTH * (1f - AOE_HIT_POINT));
    }
}