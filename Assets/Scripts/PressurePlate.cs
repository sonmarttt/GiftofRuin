using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Plate Movement")]
    public float plateSinkAmount = 0.1f;   
    public float moveSpeed = 2f;

    [Header("Target Door")]
    public Transform targetDoor;           
    public float doorRiseAmount = 2f;      

    private Vector3 plateStartPos;
    private Vector3 plateTargetPos;
    private Vector3 doorStartPos;
    private Vector3 doorTargetPos;

    private bool isPressed = false;

    void Start()
    {
        plateStartPos = transform.position;
        plateTargetPos = plateStartPos - new Vector3(0, plateSinkAmount, 0);

        if (targetDoor != null)
        {
            doorStartPos = targetDoor.position;
            doorTargetPos = doorStartPos + new Vector3(0, doorRiseAmount, 0);
        }
    }

    void Update()
    {
        // Move the plate
        Vector3 platePos = isPressed ? plateTargetPos : plateStartPos;
        transform.position = Vector3.MoveTowards(transform.position, platePos, moveSpeed * Time.deltaTime);

        // Move the door
        if (targetDoor != null)
        {
            Vector3 doorPos = isPressed ? doorTargetPos : doorStartPos;
            targetDoor.position = Vector3.MoveTowards(targetDoor.position, doorPos, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // aadd an Enemy tag to identify the npcs later
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            isPressed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy") )
        {
            isPressed = false;
        }
    }
}