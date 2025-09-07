using UnityEngine;
public class TripObstacle : MonoBehaviour
{
    [SerializeField] bool tripObstacle;

    public GameObject player;
    public PlayerController playerController;

    void Awake()
    {       
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tripObstacle)
        {
            if (tripObstacle)
            {
                playerController.speed = playerController.speed / 2; //second or two and if hits two before time is up then dies
                Destroy(gameObject);
            }
        }
    }
}
