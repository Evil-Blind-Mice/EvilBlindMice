using UnityEngine;
using System.Collections;
public class TripObstacle : MonoBehaviour
{
    [SerializeField] bool tripObstacle;
    [SerializeField] bool killObstacle;
    [SerializeField] bool spaceWorm; 
    public GameObject player;
    public PlayerStats playerStats;
    public float wormDamage = 0.25f;
    void Start()
    {
        
        player = GameObject.FindWithTag("Player");
        playerStats = PlayerStats.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tripObstacle)
        {
            playerStats.tripCounter++;
            playerStats.runSpeed = playerStats.runSpeed / 2;
            if(spaceWorm == true)
            {
                IDamage damage = other.GetComponent<IDamage>();
                damage.TakeDamage((int)(wormDamage * playerStats.maxHealth));
            }
            StartCoroutine(resetStats());
            if (playerStats.tripCounter > 1)
            {
                playerStats.runSpeed = 0;
                playerStats.currentHealth = 0;
                GameManager.instance.YouLose();
            }
        }

        if (other.CompareTag("Player") && killObstacle)
        {
            playerStats.runSpeed = 0;
            playerStats.currentHealth = 0;
            Destroy(gameObject);
            GameManager.instance.YouLose();
        }
    }

    public void RequestSpeedBoost(float _multiplier, int _durationSeconds)
    {
        if (_multiplier < 1f)
            _multiplier = 1f;

        if (_durationSeconds < 0)
            _durationSeconds = 0;

    }
    IEnumerator resetStats()
    {
        yield return new WaitForSeconds(2f);
        playerStats.runSpeed *= 2;
        playerStats.tripCounter = 0;
        Destroy(gameObject);
    }
}
