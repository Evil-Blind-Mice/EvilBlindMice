using UnityEngine;

public class ApplyUpgrades : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerShooting shooter;
    [SerializeField] PowerUpStats healPowerup;

    private void Awake()
    {
        if (!stats) stats = FindAnyObjectByType<PlayerStats>();
        if (!shooter) shooter = FindAnyObjectByType<PlayerShooting>();

        // Max Health
        if (UpgradeBank.maxHealthDelta != 0)
            stats?.AddMaxHealth(UpgradeBank.maxHealthDelta);

        // Initial Dashes
        if (UpgradeBank.initialDashDelta != 0)
            stats?.AddInitialDashCount(UpgradeBank.initialDashDelta);

        // Health from powerup
        if (healPowerup && UpgradeBank.healAmountDelta != 0)
            healPowerup.healAmount += UpgradeBank.healAmountDelta;

        // Weapons
        foreach (WeaponStats weapon in UpgradeBank.unlockedWeapons)
            if (weapon && shooter && !shooter.weaponList.Contains(weapon))
                shooter.weaponList.Add(weapon);

        GameManager.instance?.UpdatePlayerUI();
        UpgradeBank.Clear();
    }
}
