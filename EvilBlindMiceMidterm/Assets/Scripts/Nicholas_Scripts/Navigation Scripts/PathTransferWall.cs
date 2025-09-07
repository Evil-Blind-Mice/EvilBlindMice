using Unity.VisualScripting;
using UnityEngine;

public class PathTransferWall : MonoBehaviour
{
    GameObject player;
    NicholasPlayerMovement playerMovement;
    NicholasWallRunMovementState wallRunMoveState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<NicholasPlayerMovement>();
        wallRunMoveState = player.GetComponent<NicholasWallRunMovementState>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.transform.eulerAngles.z == 270)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y - 90, player.transform.rotation.eulerAngles.z);
                playerMovement.gravityDirection = -player.transform.up;
            }
            else if (player.transform.eulerAngles.z == 90)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 90, player.transform.rotation.eulerAngles.z);
                playerMovement.gravityDirection = -player.transform.up;
            }

            if (Mathf.Approximately((player.transform.eulerAngles.z), 330) && wallRunMoveState.isWallRunning == true)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y - 90, player.transform.rotation.eulerAngles.z);
            }
            else if (Mathf.Approximately((player.transform.eulerAngles.z), 30) && wallRunMoveState.isWallRunning == true)
            {
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 90, player.transform.rotation.eulerAngles.z);
            }
        }
    }
}
