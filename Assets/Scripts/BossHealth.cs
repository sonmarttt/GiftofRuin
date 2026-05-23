using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject healthBarUI;
    public TextMeshProUGUI bossNameText;

    private BossAI bossAI;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<BossAI>();
        animator = GetComponent<Animator>();

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = 1;
            healthBar.value = 1;
        }

        if (bossNameText != null)
            bossNameText.text = "Fallen Angel";
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (!bossAI.IsLanded())
        {
            Debug.Log("Boss not landed, cannot damage");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

void Die()
{
    isDead = true;
    Debug.Log("Boss died");

    bossAI.StopAllCoroutines();
    bossAI.enabled = false;

    BossAnimatorController bac = GetComponent<BossAnimatorController>();
    if (bac != null)
    {
        bac.StopAllCoroutines();
        bac.enabled = false;
    }

    animator.SetBool("isDead", true);

    if (healthBarUI != null)
        healthBarUI.SetActive(false);

    Collider col = GetComponent<Collider>();
    if (col != null) col.enabled = false;

    StartCoroutine(FallToGround());
}

IEnumerator FallToGround()
{
    float t = 0f;
    Vector3 startPos = transform.position;
    Vector3 endPos = new Vector3(transform.position.x, -3f, transform.position.z); // adjust -2f to match your ground

    while (t < 1f)
    {
        t += Time.deltaTime * 0.8f; // slow fall
        transform.position = Vector3.Lerp(startPos, endPos, t);
        yield return null;
    }
}
}