using UnityEngine;

public class PathTransferWall : MonoBehaviour
{
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.transform.eulerAngles.z == -90 || player.transform.eulerAngles.z == 270)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y - 90, player.transform.rotation.eulerAngles.z);
                player.GetComponent<PlayerMovement>().gravityDirection = -player.transform.up;
            }
            else if (player.transform.eulerAngles.z == 90 || player.transform.eulerAngles.z == -270)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 90, player.transform.rotation.eulerAngles.z);
                player.GetComponent<PlayerMovement>().gravityDirection = -player.transform.up;
            }

            if (player.transform.eulerAngles.z == -30 || player.transform.eulerAngles.z == 330)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y - 90, player.transform.rotation.eulerAngles.z);
                player.GetComponent<PlayerMovement>().gravityDirection = -player.transform.up;
            }
            else if (player.transform.eulerAngles.z == 30 || player.transform.eulerAngles.z == -210)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 90, player.transform.rotation.eulerAngles.z);
                player.GetComponent<PlayerMovement>().gravityDirection = -player.transform.up;
            }
        }
    }
}
