using UnityEngine;
using System.Collections;
public class TripObstacle : MonoBehaviour
{
    [SerializeField] bool tripObstacle;

    public GameObject player;
    public PlayerController playerController;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
    }
    //IEnumerator revertAfterSeconds(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);
    //    playerController.speed = playerController.speed * 2;
    //}

}
