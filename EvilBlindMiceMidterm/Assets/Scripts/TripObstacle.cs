using UnityEngine;
using System.Collections;
public class TripObstacle : MonoBehaviour
{
    [SerializeField] bool tripObstacle;
    [SerializeField] bool killObstacle;
    public GameObject player;
    public PlayerStats PlayerStats;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        PlayerStats = player.GetComponent<PlayerStats>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tripObstacle)
        {
            PlayerStats.hasTripped++;
            PlayerStats.runSpeed = PlayerStats.runSpeed / 2; 
            resetStats();
            
            Destroy(gameObject);
        }

        if (other.CompareTag("Player") && killObstacle)
        {
            PlayerStats.runSpeed = 0;
            PlayerStats.currentHealth = 0;
            Destroy(gameObject);
        }
    }

    IEnumerator resetStats()
    {
        if (PlayerStats.hasTripped > 1)
        {
            PlayerStats.runSpeed = 0;
            PlayerStats.currentHealth = 0;
        }
        else { 
        yield return new WaitForSeconds(2f);
            PlayerStats.runSpeed = PlayerStats.initialRunSpeed;
        }
    }
}
