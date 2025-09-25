using UnityEngine;
using System.Collections;
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

    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip[] audioChangeWeapon;
    [SerializeField, Range(0, 1)] float audioChangeWeaponVolume;

    Coroutine reloadRoutine;

    public bool InfiniteAmmoActive => infiniteAmmoActive;

    bool isReloading;
    bool isPlayingReload;

    float shootTimer;
    float infiniteAmmoRemaining;

    public int weaponListPosition;

    private void Awake()
    {
        instance = this;
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

    public void Shoot()
    {
        shootTimer = 0;

        if (isReloading) return;
        if (weaponList.Count == 0) return;
        WeaponStats weapon = weaponList[weaponListPosition];

        if (!infiniteAmmoActive)
        {
            if (weapon.weaponCurrentAmmo <= 0) return;
            weapon.weaponCurrentAmmo--;
        }

        audio.PlayOneShot(weaponList[weaponListPosition].shootingSound
            [Random.Range(0, weaponList[weaponListPosition].shootingSound.Length)],
            weaponList[weaponListPosition].shootingSoundVolume);

        GameManager.instance?.UpdatePlayerUI();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, weaponFiringDistance, ~ignoreLayers))
        {
            Instantiate(weaponList[weaponListPosition].hitEffect, hit.point, Quaternion.identity);

            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null)
                damage.TakeDamage(weaponFiringDamage);
        }
    }

    void ReloadWeapon()
    {
        if (weaponList.Count == 0) return;
        if (infiniteAmmoActive) return;

        if (Input.GetButtonDown("Reload"))
        {
            WeaponStats weapon = weaponList[weaponListPosition];
            if (weapon.weaponCurrentAmmo < weapon.weaponMaxAmmo && !isReloading)
            {
                reloadRoutine = StartCoroutine(PlayReload());
            }
        }
    }

    void SelectWeapon()
    {
        if (weaponList.Count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0)) return;

        int oldIndex = weaponListPosition;
        int nextIndex = oldIndex + (scroll > 0 ? 1 : -1);
        nextIndex = Mathf.Clamp(nextIndex, 0, weaponList.Count - 1);

        if (nextIndex == oldIndex) return;

        if (Mathf.Min(weaponListPosition + 1, weaponList.Count - 1) == Mathf.Max(weaponListPosition - 1, 0)) return;

        if (isReloading && reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
            isReloading = false;
        }

        weaponListPosition = nextIndex;

        audio.PlayOneShot(audioChangeWeapon[Random.Range(0, audioChangeWeapon.Length)], audioChangeWeaponVolume);
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

        if (weaponModel != null && weapon.weaponModel != null)
        {
            MeshFilter sourceMeshFilter = weapon.weaponModel ? weapon.weaponModel.GetComponentInChildren<MeshFilter>() : null;
            MeshRenderer sourceMeshRenderer = weapon.weaponModel ? weapon.weaponModel.GetComponentInChildren<MeshRenderer>() : null;

            MeshFilter destinationMeshFilter = weaponModel.GetComponent<MeshFilter>();
            MeshRenderer destinationMeshRenderer = weaponModel.GetComponent<MeshRenderer>();

            if (sourceMeshFilter && destinationMeshFilter)
                destinationMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;

            if (sourceMeshRenderer && destinationMeshRenderer)
                destinationMeshRenderer.sharedMaterial = sourceMeshRenderer.sharedMaterial;
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

    IEnumerator PlayReload()
    {
        isReloading = true;

        WeaponStats weapon = weaponList[weaponListPosition];
        AudioClip clip = weapon.reloadSound[Random.Range(0, weapon.reloadSound.Length)];

        audio.PlayOneShot(clip, weapon.reloadSoundVolume);

        yield return new WaitForSeconds(clip.length);

        weapon.weaponCurrentAmmo = weapon.weaponMaxAmmo;
        GameManager.instance?.UpdatePlayerUI();

        isReloading = false;
        reloadRoutine = null;
    }
}
