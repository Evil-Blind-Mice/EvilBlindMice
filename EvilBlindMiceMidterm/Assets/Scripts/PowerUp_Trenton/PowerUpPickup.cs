using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] PowerUpStats powerUp;

    private void OnTriggerEnter(Collider _other)
    {
        IPickupPowerUp pickupable = _other.GetComponent<IPickupPowerUp>();

        if (pickupable != null)
        {
            pickupable.GetPowerUpStats(powerUp);
            Destroy(gameObject);
        }
    }
}
