using UnityEngine;
using System.Collections;
public class TripObstacle : MonoBehaviour
{
    [SerializeField] bool tripObstacle;
    [SerializeField] bool killObstacle;
    public GameObject player;
    public PlayerController playerController;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tripObstacle)
        {
            playerController.hasTripped++;
            //StartCoroutine(revertAfterSeconds(2f));
            playerController.speed = playerController.speed / 2; //second or two and if hits two before time is up then dies
            if (playerController.hasTripped > 1)
            {
                playerController.speed = 0;
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Player") && killObstacle)
        {
            playerController.speed = 0;
            playerController.health = 0;
        }
    }
    //IEnumerator revertAfterSeconds(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);
    //    playerController.speed = playerController.speed * 2;
    //}
}
