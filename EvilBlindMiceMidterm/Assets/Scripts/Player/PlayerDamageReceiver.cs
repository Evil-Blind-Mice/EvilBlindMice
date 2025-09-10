using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IDamage
{
    PlayerStats playerStats;

    void Awake()
    {
        playerStats = PlayerStats.instance;
    }

    public void TakeDamage(int _amount)
    {
        if (_amount <= 0) return;
        if (playerStats == null) return;
        if (playerStats.IsInvincible()) return;

        int currentHealth = playerStats.GetHealth();
        if (currentHealth <= 0) return;

        int newHealth = Mathf.Max(0, currentHealth - _amount);
        playerStats.AddHealth(newHealth - currentHealth);
    }
}
