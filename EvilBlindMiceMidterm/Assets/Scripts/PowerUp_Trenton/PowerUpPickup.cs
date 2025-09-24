using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] PowerUpStats powerup;

    private void OnTriggerEnter(Collider _other)
    {
        IPickupPowerUp pickupable = _other.GetComponent<IPickupPowerUp>();

        if (pickupable != null)
        {
            pickupable.GetPowerUpStats(powerup);
            PlayPickupAudio();
            Destroy(gameObject);
        }
    }

    void PlayPickupAudio()
    {
        if (!powerup) return;
        Vector3 soundPosition = transform.position;

        if (powerup.pickupSound != null && powerup.pickupSound.Length > 0)
        {
            AudioClip clip = powerup.pickupSound[0];
            if (clip)
                AudioSource.PlayClipAtPoint(clip, soundPosition, Mathf.Clamp01(powerup.pickupSoundVolume));
        }
    }
}
