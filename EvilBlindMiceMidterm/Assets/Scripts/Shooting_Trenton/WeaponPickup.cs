using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] WeaponStats weapon;

    private void OnTriggerEnter(Collider _other)
    {
        IPickupWeapon pickupable = _other.GetComponent<IPickupWeapon>();

        if (pickupable != null)
        {
            weapon.weaponCurrentAmmo = weapon.weaponMaxAmmo;
            pickupable.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
