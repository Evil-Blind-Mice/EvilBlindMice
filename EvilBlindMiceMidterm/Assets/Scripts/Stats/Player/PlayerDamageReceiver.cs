using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask groundLayer;
    [Tooltip("The distance above the player's feet that the raycast should originate from when checking for head-on collisions")]
    [SerializeField] float crashCastOffset = 0.5f;
    [SerializeField] int gameOverDamage;
    [SerializeField] float impactDistance = .75f;

    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip[] audioHurt;
    [SerializeField, Range(0, 1)] float audioHurtVolume; 

    PlayerStats playerStats;

    void Start()
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

        audio.PlayOneShot(audioHurt[Random.Range(0, audioHurt.Length)], audioHurtVolume);

        int newHealth = Mathf.Max(0, currentHealth - _amount);
        playerStats.AddHealth(newHealth - currentHealth);
    }

    private void Update()
    {
        Debug.DrawRay(transform.position + transform.up * crashCastOffset, transform.forward * impactDistance, Color.red);
        if (Physics.Raycast(transform.position + transform.up * crashCastOffset, transform.forward, impactDistance, groundLayer))
        { // ran face first into a wall
            TakeDamage(gameOverDamage);
        }
    }

}


