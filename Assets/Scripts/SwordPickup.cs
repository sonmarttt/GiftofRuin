using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    [Header("Ground Swords (visible on ground)")]
    public GameObject groundSwords;

    [Header("Equipped Swords (parented to hands)")]
    public GameObject sword1; // Sword7_FBX
    public GameObject sword2; // Sword3_FBX

    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.E;

    private Transform player;
    private bool canPickup = false;
    private bool pickedUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (groundSwords != null) groundSwords.SetActive(false);
        if (sword1 != null) sword1.SetActive(false);
        if (sword2 != null) sword2.SetActive(false);
    }

    void Update()
    {
        if (pickedUp) return;

        float dist = Vector3.Distance(transform.position, player.position);
        canPickup = dist <= pickupRange && groundSwords != null && groundSwords.activeSelf;

        if (canPickup && Input.GetKeyDown(pickupKey))
            Pickup();
    }

    public void RevealOnGround()
    {
        if (groundSwords != null)
            groundSwords.SetActive(true);
    }

    void Pickup()
    {
        pickedUp = true;
        if (groundSwords != null) groundSwords.SetActive(false);
        if (sword1 != null) sword1.SetActive(true);
        if (sword2 != null) sword2.SetActive(true);
    }

    void OnGUI()
    {
        if (canPickup && !pickedUp)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 30),
                "[E] Pick up Dual Swords", style);
        }
    }
}