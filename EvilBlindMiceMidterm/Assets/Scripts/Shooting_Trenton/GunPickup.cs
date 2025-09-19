using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] WeaponStats weapon;

    private void OnTriggerEnter(Collider _other)
    {
        IPickupWeapon pickupable = _other.GetComponent<IPickupWeapon>();

        if (pickupable != null)
        {
            pickupable.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
