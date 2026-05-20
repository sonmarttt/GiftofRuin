using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [Header("Scrolling")]
    public float scrollSpeedX = 0.05f;
    public float scrollSpeedY = 0.02f;

    [Header("Respawn")]
    public Transform respawnPoint;   
                                     

    private Material mat;
    private Vector3 playerStartPos;
    private bool startCaptured = false;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        mat.mainTextureOffset = new Vector2(
            Time.time * scrollSpeedX,
            Time.time * scrollSpeedY
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        TryRespawn(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryRespawn(other.gameObject);
    }

    void TryRespawn(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        // Capture the player's starting position the first time we see them.
        if (!startCaptured)
        {
            playerStartPos = obj.transform.position;
            startCaptured = true;
        }

        Vector3 target = respawnPoint != null ? respawnPoint.position : playerStartPos;

        // Zero out velocity so they don't keep falling after teleport.
        Rigidbody prb = obj.GetComponent<Rigidbody>();
        if (prb != null)
        {
            prb.velocity  = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
        }

        // CharacterController must be disabled before moving, or it fights the teleport.
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            obj.transform.position = target;
            cc.enabled = true;
        }
        else
        {
            obj.transform.position = target;
        }
    }
}