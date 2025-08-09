using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    public List<GameObject> weaponPrefabs;
    public Transform weaponHolder;
    private GameObject currentWeapon;
    private int currentIndex = 0;

    void Start()
    {
        SpawnWeapon(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapWeapon();
        }
    }

    void SwapWeapon()
    {
        Destroy(currentWeapon);
        currentIndex = (currentIndex + 1) % weaponPrefabs.Count;
        SpawnWeapon(currentIndex);
    }

    void SpawnWeapon(int index)
    {
        currentWeapon = Instantiate(weaponPrefabs[index], weaponHolder.position, weaponHolder.rotation, weaponHolder);

        GunController gun = currentWeapon.GetComponent<GunController>();
        if (gun != null)
        {
            gun.UpdateAmmoUI();
        }
    }

}
