using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySceneManager : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Boss")]
    public BossHealth bossHealth;

    private Transform player;
    private bool canPickup = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (bossHealth == null)
            bossHealth = FindObjectOfType<BossHealth>();
    }

    void Update()
    {
        if (bossHealth != null && !bossHealth.IsDead) return;

        float dist = Vector3.Distance(transform.position, player.position);
        canPickup = dist <= interactRange;

        if (canPickup && Input.GetKeyDown(interactKey))
            SceneManager.LoadScene("Victory Scene");
    }

    void OnGUI()
    {
        if (bossHealth != null && !bossHealth.IsDead) return;
        if (!canPickup) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 30),
            "[E] Pick up the Artifact", style);
    }
}