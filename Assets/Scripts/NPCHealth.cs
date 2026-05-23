using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NPCHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Health Bar")]
    public Canvas healthBarCanvas;   // the World Space canvas on the NPC
    public Image healthBarFill;      // the filled Image inside it

    private Transform cam;

    void Start()
    {
        currentHealth = maxHealth;
        cam = Camera.main.transform;
        UpdateHealthBar();
    }

    void LateUpdate()
    {
        // Keep bar facing camera
        if (healthBarCanvas != null)
            healthBarCanvas.transform.LookAt(
                healthBarCanvas.transform.position + cam.forward
            );
    }

    public void TakeHit()  => TakeDamage(34); // ~3 hits to kill

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;   // must be first
        GetComponent<NPCAnimator>().enabled = false;
        GetComponent<GrabbableCorpse>().Die();          // then this
        this.enabled = false;

        if (SceneManager.GetActiveScene().name == "SampleScene") {
            StartCoroutine(ExecuteAfterTime(3f));
        }
    }

    IEnumerator ExecuteAfterTime(float time) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Level1");
    }
}