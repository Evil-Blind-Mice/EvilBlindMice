using Unity.VisualScripting;
using UnityEngine;

public class PlayerPowerUps : MonoBehaviour, IPickupPowerUp
{
    public void GetPowerUpStats(PowerUpStats powerUp)
    {
        if (powerUp == null || PlayerStats.instance == null) return;

        switch (powerUp.type)
        {
            case PowerUpType.Heal:
                int missing = PlayerStats.instance.GetMaxHealth() - PlayerStats.instance.GetHealth();
                if (missing > 0)
                    PlayerStats.instance.AddHealth(Mathf.Min(powerUp.healAmount, missing));
                break;

            case PowerUpType.SpeedBoost:
                PlayerStats.instance.RequestSpeedBoost(powerUp.speedMultiplier, powerUp.speedDurationSeconds);
                GameManager.instance.FlashSpeedBoost(powerUp.speedDurationSeconds);
                break;

            case PowerUpType.Invincibility:
                PlayerStats.instance.RequestInvincibility(powerUp.invincibilityDurationSeconds);
                break;

            case PowerUpType.TimeSlow:
                TimeSlowService.Apply(powerUp.slowScale, powerUp.slowDurationSeconds);
                break;

            case PowerUpType.Trip:
                PlayerStats.instance.RequestTripState(powerUp.tripSpeedDivider, powerUp.tripDurationSeconds);
                break;
        }

        GameManager.instance.UpdatePlayerUI();
    }
}
