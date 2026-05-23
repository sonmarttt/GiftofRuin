// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MovingPlatform : MonoBehaviour
// {
//     [Header("Movement")]
//     public bool moveEnabled = true;
//     public float speed = 2f;

//     public float leftRightDistance = 3f; 
//     public float forwardBackDistance = 0f; 

//     [Header("Collapse")]
//     public bool collapseEnabled = true;
//     public float collapseAfterSeconds = 3f;  


//     private Vector3 startPos;
//     private bool playerOnPlatform = false;
//     private float timeOnPlatform = 0f;
//     private bool collapsed = false;
//     private Rigidbody rb;

//     void Start()
//     {
//         startPos = transform.position;
//         rb = GetComponent<Rigidbody>();
//         if (rb == null)
//         {
//             rb = gameObject.AddComponent<Rigidbody>();
//         }
//         rb.isKinematic = true;
//         rb.useGravity = false;
//     }

//     void Update()
//     {
//         if (moveEnabled && !collapsed)
//         {
//             float offsetZ = Mathf.Sin(Time.time * speed) * leftRightDistance;
//             float offsetX = Mathf.Sin(Time.time * speed) * forwardBackDistance;
//             transform.position = startPos + new Vector3(offsetX, 0f, offsetZ);
//         }

//         if (collapseEnabled && playerOnPlatform && !collapsed)
//         {
//             timeOnPlatform += Time.deltaTime;
//             if (timeOnPlatform >= collapseAfterSeconds)
//             {
//                 Collapse();
//             }
//         }
//     }

//     void Collapse()
//     {
//         collapsed = true;
//         rb.isKinematic = false;   
//         rb.useGravity = true;     

//     }

//     void ResetPlatform()
//     {
//         rb.isKinematic = true;
//         rb.useGravity = false;
//         transform.position = startPos;
//         timeOnPlatform = 0f;
//         playerOnPlatform = false;
//         collapsed = false;
//     }

//     void OnCollisionEnter(Collision collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         playerOnPlatform = true;
//         collision.transform.SetParent(transform);   
//     }
// }

// void OnCollisionExit(Collision collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         playerOnPlatform = false;
//         collision.transform.SetParent(null);       
//     }
// }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public bool moveEnabled = true;
    public float speed = 2f;
    public float leftRightDistance = 3f;
    public float forwardBackDistance = 0f;

    [Header("Collapse")]
    public bool collapseEnabled = true;
    public float collapseAfterSeconds = 3f;

    private Vector3 startPos;
    private bool playerOnPlatform = false;
    private float timeOnPlatform = 0f;
    private bool collapsed = false;
    private Rigidbody rb;

    private Rigidbody playerRb;        // The player's Rigidbody
    private Vector3 lastPlatformPosition;

    void Start()
    {
        startPos = transform.position;
        lastPlatformPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        if (moveEnabled && !collapsed)
        {
            float offsetZ = Mathf.Sin(Time.time * speed) * leftRightDistance;
            float offsetX = Mathf.Sin(Time.time * speed) * forwardBackDistance;
            transform.position = startPos + new Vector3(offsetX, 0f, offsetZ);
        }

        if (collapseEnabled && playerOnPlatform && !collapsed)
        {
            timeOnPlatform += Time.deltaTime;
            if (timeOnPlatform >= collapseAfterSeconds)
                Collapse();
        }
    }

    void FixedUpdate()
    {
        // Apply platform delta to the player's Rigidbody in FixedUpdate
        // to stay in sync with the player's own physics updates
        if (playerOnPlatform && !collapsed && playerRb != null)
        {
            Vector3 delta = transform.position - lastPlatformPosition;
            playerRb.MovePosition(playerRb.position + delta);
        }

        lastPlatformPosition = transform.position;
    }

    void Collapse()
    {
        collapsed = true;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void ResetPlatform()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = startPos;
        lastPlatformPosition = startPos;
        timeOnPlatform = 0f;
        playerOnPlatform = false;
        collapsed = false;
        playerRb = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            // Grab the Rigidbody directly instead of storing the Transform
            playerRb = collision.gameObject.GetComponent<Rigidbody>();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            playerRb = null;
        }
    }
}