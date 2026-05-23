using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySceneManager : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    private Transform player;
    private PlayerMovement playerMovement;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyDown(interactKey))
        {
            if (dist <= interactRange)
            {
                SceneManager.LoadScene("Victory Scene");
            }
        }
    }
}
