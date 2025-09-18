using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] WeaponStats gun;

    private void OnTriggerEnter(Collider _other)
    {
        IPickupWeapon pickupable = _other.GetComponent<IPickupWeapon>();

        if (pickupable != null)
        {
            pickupable.GetWeaponStats(gun);
            Destroy(gameObject);
        }
    }
}
