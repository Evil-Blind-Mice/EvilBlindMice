using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] WeaponStats gun;

    private void OnTriggerEnter(Collider _other)
    {
        IPickup pickupable = _other.GetComponent<IPickup>();

        if (pickupable != null )
        {
            gun.currentWeaponAmmo = gun.maxWeaponAmmo;
            pickupable.GetWeaponStats(gun);
            Destroy(gameObject);
        }
    }
}
