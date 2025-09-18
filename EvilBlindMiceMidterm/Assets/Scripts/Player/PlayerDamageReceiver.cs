using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IDamage
{
    PlayerStats playerStats;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int gameOverDamage;

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

    private void OnCollisionEnter(Collision _collision)
    {
        
        if(Physics.Raycast(transform.position, transform.forward, 1, groundLayer))
        { // ran face first into a wall
            TakeDamage(gameOverDamage);
        }
           
    }
}


