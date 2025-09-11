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
            PlayerStats.tripCounter++;
            PlayerStats.runSpeed = PlayerStats.runSpeed / 2;
            StartCoroutine(resetStats());
            if (PlayerStats.tripCounter > 1)
            {
                PlayerStats.runSpeed = 0;
                PlayerStats.currentHealth = 0;
                GameManager.instance.YouLose();
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Player") && killObstacle)
        {
            PlayerStats.runSpeed = 0;
            PlayerStats.currentHealth = 0;
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
        PlayerStats.runSpeed = PlayerStats.initialRunSpeed;
    }
}
