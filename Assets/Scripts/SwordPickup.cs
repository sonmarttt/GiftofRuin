using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    [Header("Ground Swords (visible on ground)")]
    public GameObject groundSwords;

    [Header("Weapon Manager")]
    public WeaponManager weaponManager;
    public int weaponIndex; // 0 = dual swords, 1 = single sword

    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.E;

    private Transform player;
    private bool canPickup = false;
    private bool pickedUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (groundSwords != null) groundSwords.SetActive(false);
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

        if (weaponManager != null)
            weaponManager.UnlockWeapon(weaponIndex);
    }

    void OnGUI()
    {
        if (canPickup && !pickedUp)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 30),
                "[E] Pick up " + (weaponIndex == 0 ? "Dual Swords" : "Sword"), style);
        }
    }
}