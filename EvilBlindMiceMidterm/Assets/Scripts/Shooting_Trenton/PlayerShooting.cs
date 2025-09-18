using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour, IPickupWeapon
{
    [Header("Gun")]
    [SerializeField] LayerMask ignoreLayers;

    [SerializeField] List<WeaponStats> weaponList = new List<WeaponStats>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] int weaponFiringDamage;
    [SerializeField] float weaponFireRate;
    [SerializeField] int weaponFiringDistance;

    private readonly Dictionary<WeaponStats, int> ammoByWeapon = new();

    WeaponStats currentWeapon;

    float shootTimer;

    int weaponListPosition;
    int weaponCurrentAmmo, weaponMaxAmmo;

    public bool HasWeapon => currentWeapon != null;
    public int WeaponCurrentAmmo => weaponCurrentAmmo;
    public int WeaponMaxAmmo => weaponMaxAmmo;

    void Update()
    {
        shootTimer += Time.deltaTime;

        bool hasWeapon = weaponList != null && weaponList.Count > 0;
        bool canShoot = hasWeapon && weaponCurrentAmmo > 0 && shootTimer >= weaponFireRate;

        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        SelectWeapon();
        ReloadWeapon();

        if (Input.GetButton("Fire1") && canShoot)
            Shoot();
    }

    public void Shoot()
    {
        shootTimer = 0;
        weaponCurrentAmmo--;
        ammoByWeapon[currentWeapon] = weaponCurrentAmmo;
        GameManager.instance.UpdatePlayerUI();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, weaponFiringDistance, ~ignoreLayers))
        {
            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null)
                damage.TakeDamage(weaponFiringDamage);
        }
    }

    void ReloadWeapon()
    {
        if (Input.GetButtonDown("Reload") && (weaponCurrentAmmo < weaponMaxAmmo))
        {
            weaponCurrentAmmo = weaponMaxAmmo;
            ammoByWeapon[currentWeapon] = weaponCurrentAmmo;

            GameManager.instance.UpdatePlayerUI();
        }

    }

    void SelectWeapon()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0)) return;

        if (scroll > 0)
            weaponListPosition = (weaponListPosition + 1) % weaponList.Count;
        else if (scroll < 0)
            weaponListPosition = (weaponListPosition - 1 + weaponList.Count) % weaponList.Count;

        ChangeWeapon();
    }

    public void GetWeaponStats(WeaponStats _weapon)
    {
        if (!ammoByWeapon.ContainsKey(_weapon))
            ammoByWeapon[_weapon] = _weapon.weaponMaxAmmo;

        weaponList.Add(_weapon);
        weaponListPosition = weaponList.Count - 1;

        ChangeWeapon();
    }

    void ChangeWeapon()
    {
        currentWeapon = weaponList[weaponListPosition];

        weaponFiringDamage = currentWeapon.weaponFiringDamage;
        weaponFiringDistance = currentWeapon.weaponFiringDistance;
        weaponFireRate = currentWeapon.weaponFireRate;

        weaponMaxAmmo = currentWeapon.weaponMaxAmmo;
        weaponCurrentAmmo = ammoByWeapon[currentWeapon];

        weaponModel.GetComponent<MeshFilter>().sharedMesh = currentWeapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = currentWeapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;

        GameManager.instance.UpdatePlayerUI();
    }
}
