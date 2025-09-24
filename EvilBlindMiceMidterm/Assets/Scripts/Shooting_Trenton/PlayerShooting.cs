using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour, IPickupWeapon
{
    public static PlayerShooting instance;

    [Header("Gun")]
    [SerializeField] LayerMask ignoreLayers;

    [SerializeField] public List<WeaponStats> weaponList = new List<WeaponStats>();
    [SerializeField] WeaponStats startingWeapon;
    [SerializeField] GameObject weaponModel;
    [SerializeField] int weaponFiringDamage;
    [SerializeField] float weaponFireRate;
    [SerializeField] int weaponFiringDistance;
    [SerializeField] bool infiniteAmmoActive;

    [Header("Projectile")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectile;
    [SerializeField, Min(1)] float projectileSpeed;
    [SerializeField, Min(0.05f)] float projectileLifeSeconds;

    Transform currentWeaponInstance;
    Camera shootCamera;

    public bool InfiniteAmmoActive => infiniteAmmoActive;

    float shootTimer;
    float infiniteAmmoRemaining;

    public int weaponListPosition;

    private void Awake()
    {
        instance = this;
        shootCamera = Camera.main;
    }

    private void Start()
    {
        EnsureStartingWeapon();
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        if (infiniteAmmoActive)
        {
            infiniteAmmoRemaining -= Time.unscaledDeltaTime;
            if (infiniteAmmoRemaining <= 0)
            {
                infiniteAmmoActive = false;
                infiniteAmmoRemaining = 0;
            }
        }

        SelectWeapon();
        ReloadWeapon();

        int weaponPosition = Mathf.Clamp(weaponListPosition, 0, weaponList.Count - 1);
        WeaponStats weapon = weaponList[weaponPosition];

        bool wantsToShoot = weapon.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        bool canShoot = weapon.weaponCurrentAmmo > 0 && shootTimer >= weaponFireRate;

        if (wantsToShoot && canShoot)
            Shoot();

        
    }

    void EnsureStartingWeapon()
    {
        if (startingWeapon == null) return;

        weaponList.Clear();

        weaponList.Add(startingWeapon);
        weaponListPosition = 0;

        startingWeapon.weaponCurrentAmmo = startingWeapon.weaponMaxAmmo;

        ChangeWeapon();
        GameManager.instance?.UpdatePlayerUI();
    }

    static void SetLayer(Transform _transform, int _layer)
    {
        _transform.gameObject.layer = _layer;
        foreach (Transform c in _transform)
            SetLayer(c, _layer);
    }

    public void Shoot()
    {
        shootTimer = 0;

        if (weaponList.Count == 0) return;
        WeaponStats weapon = weaponList[weaponListPosition];

        if (!infiniteAmmoActive)
        {
            if (weapon.weaponCurrentAmmo <= 0) return;
            weapon.weaponCurrentAmmo--;
        }

        GameManager.instance?.UpdatePlayerUI();

        if (!projectile) return;
        if (!shootCamera) shootCamera = Camera.main;

        Vector3 spawnPosition;
        Vector3 direction;
        Quaternion rot;

        if(firePoint)
        {
            spawnPosition = firePoint.position;
            direction = firePoint.forward;
            rot = firePoint.rotation;
        }
        else
        {
            spawnPosition = shootCamera.transform.position + shootCamera.transform.forward * 0.4f;
            direction = shootCamera.transform.forward;
            rot = Quaternion.LookRotation(direction);
        }

        float life = Mathf.Max(projectileLifeSeconds, weaponFiringDistance / Mathf.Max(1, projectileSpeed));

        GameObject go = Instantiate(projectile, spawnPosition, rot);

        int bulletLayer = LayerMask.NameToLayer("PlayerBullet");
        SetLayer(go.transform, bulletLayer);

        LaserProjectile laser = go.GetComponent<LaserProjectile>();
        if (laser)
            laser.Init(direction, projectileSpeed, life, weaponFiringDamage, gameObject, weapon);

        if (weapon.shootingSound != null && weapon.shootingSound.Length > 0)
        {
            AudioClip clip = weapon.shootingSound[Random.Range(0, weapon.shootingSound.Length)];
            if (clip)
            {
                Vector3 muzzle = firePoint ? firePoint.position : (Camera.main ? Camera.main.transform.position : transform.position);
                AudioSource.PlayClipAtPoint(clip, muzzle, Mathf.Clamp01(weapon.shootingSoundVolume));
            }
        }
    }

    void ReloadWeapon()
    {
        if (weaponList.Count == 0) return;

        if (infiniteAmmoActive) return;

        if (Input.GetButtonDown("Reload"))
        {
            WeaponStats weapon = weaponList[weaponListPosition];
            if (weapon.weaponCurrentAmmo < weapon.weaponMaxAmmo)
            {
                weapon.weaponCurrentAmmo = weapon.weaponMaxAmmo;
                GameManager.instance?.UpdatePlayerUI();
            }
        }
    }

    void SelectWeapon()
    {
        if (weaponList.Count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0)) return;

        if (scroll > 0)
            weaponListPosition = Mathf.Min(weaponListPosition + 1, weaponList.Count - 1);
        else
            weaponListPosition = Mathf.Max(weaponListPosition - 1, 0);

        ChangeWeapon();
    }

    public void GetWeaponStats(WeaponStats _weapon)
    {
        if (!weaponList.Contains(_weapon))
            weaponList.Add(_weapon);

        weaponListPosition = weaponList.Count - 1;
        ChangeWeapon();
    }

    void ChangeWeapon()
    {
        if (weaponList.Count == 0) return;

        WeaponStats weapon = weaponList[weaponListPosition];

        weaponFiringDamage = weapon.weaponFiringDamage;
        weaponFiringDistance = weapon.weaponFiringDistance;
        weaponFireRate = weapon.weaponFireRate;

        if (currentWeaponInstance)
            Destroy(currentWeaponInstance.gameObject);

        foreach (Transform child in weaponModel.transform)
            Destroy(child.gameObject);

        if (weapon.weaponModel)
        {
            GameObject go = Instantiate(weapon.weaponModel, weaponModel.transform);
            currentWeaponInstance = go.transform;

            SetLayer(currentWeaponInstance, weaponModel.gameObject.layer);

            Transform muzzle = currentWeaponInstance.Find("Muzzle");
            firePoint = muzzle ? muzzle : currentWeaponInstance;
        }

        GameManager.instance?.UpdatePlayerUI();
    }

    public void ActivateInfiniteAmmo(int _duration)
    {
        infiniteAmmoRemaining = Mathf.Max(infiniteAmmoRemaining, Mathf.Max(0, _duration));
        infiniteAmmoActive = true;

        int weaponPosition = Mathf.Clamp(weaponListPosition, 0, weaponList.Count - 1);
        WeaponStats weapon = weaponList[weaponPosition];

        if (weapon)
            weapon.weaponCurrentAmmo = weapon.weaponMaxAmmo;
    }
}
