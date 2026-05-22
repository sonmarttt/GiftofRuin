using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string weaponName;
        public GameObject[] weaponObjects;
    }

    public Weapon[] weapons;
    public KeyCode switchKey = KeyCode.Q;
    public bool startWithAllWeapons = false; // check this in Inspector for Level 1

    private int currentIndex = -1;
    private bool[] unlocked;

    public int CurrentWeaponIndex => currentIndex;

    void Start()
    {
        unlocked = new bool[weapons.Length];

        foreach (var w in weapons)
            foreach (var obj in w.weaponObjects)
                if (obj != null) obj.SetActive(false);

        if (startWithAllWeapons)
        {
            for (int i = 0; i < weapons.Length; i++)
                unlocked[i] = true;

            EquipWeapon(0); // default to dual swords
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
            CycleWeapon();
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        unlocked[index] = true;

        if (currentIndex == -1)
            EquipWeapon(index);
    }

    void CycleWeapon()
    {
        int next = currentIndex;
        for (int i = 1; i <= weapons.Length; i++)
        {
            int check = (currentIndex + i) % weapons.Length;
            if (unlocked[check])
            {
                next = check;
                break;
            }
        }

        if (next != currentIndex)
            EquipWeapon(next);
    }

    void EquipWeapon(int index)
    {
        if (currentIndex >= 0)
            foreach (var obj in weapons[currentIndex].weaponObjects)
                if (obj != null) obj.SetActive(false);

        currentIndex = index;
        foreach (var obj in weapons[currentIndex].weaponObjects)
            if (obj != null) obj.SetActive(true);

        Debug.Log("Equipped: " + weapons[currentIndex].weaponName);
    }
}