using System.Collections;
using UnityEngine;

public class BossAnimatorController : MonoBehaviour
{
    private const float AOE_LENGTH = 2.167f;
    private const float SPELL_LENGTH = 2.300f;
    private const float SPELL_FIRE_POINT = 0.357f;
    private const float AOE_HIT_POINT = 0.6f;

    private Animator animator;
    private BossAI bossAI;
    private BossHealth bossHealth;

    public delegate void OnSpellFire();
    public delegate void OnAOEHit();

    public static event OnSpellFire SpellFireEvent;
    public static event OnAOEHit AOEHitEvent;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossAI = GetComponent<BossAI>();
        bossHealth = GetComponent<BossHealth>();

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

    bool IsDead() => bossHealth != null && !bossHealth.enabled;

    IEnumerator SpellSequence()
    {
        yield return new WaitForSeconds(SPELL_LENGTH * SPELL_FIRE_POINT);
        if (IsDead()) yield break;
        SpellFireEvent?.Invoke();

        yield return new WaitForSeconds(SPELL_LENGTH * (1f - SPELL_FIRE_POINT));
    }

    IEnumerator AOESequence()
    {
        yield return new WaitForSeconds(AOE_LENGTH * AOE_HIT_POINT);
        if (IsDead()) yield break;
        AOEHitEvent?.Invoke();

        yield return new WaitForSeconds(AOE_LENGTH * (1f - AOE_HIT_POINT));
    }
}