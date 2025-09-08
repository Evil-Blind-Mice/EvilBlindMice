using Unity.VisualScripting;
using UnityEngine;

public class PathTransferWall : MonoBehaviour
{
    GameObject player;
    NicholasPlayerMovement playerMovement;
    NicholasWallRunMovementState wallRunMoveState;
    NicholasDefaultMovementState defaultMovementState;
    [SerializeField] float secondsToRotateWall = 0.25f;
    [SerializeField] float secondsToRotateTilted = 0.15f;
    bool tilted = false;
    bool rotate = false;
    float currentRotation = 0;
    float fullRotation = 0;
    int origionalGravity = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<NicholasPlayerMovement>();
        wallRunMoveState = player.GetComponent<NicholasWallRunMovementState>();
        defaultMovementState = player.GetComponent <NicholasDefaultMovementState>();
    }

    private void Update()
    {
        if (rotate)
        {
            if (currentRotation != 0)
            {
                RotateAroundY(ref currentRotation);
                if (playerMovement.maxGravity > 0)
                {
                    origionalGravity = playerMovement.maxGravity;
                    playerMovement.maxGravity = 0;
                }
                if (defaultMovementState.cancelBodyMovement == 1)
                {
                    defaultMovementState.cancelBodyMovement = 0;
                }
            }
            else if (currentRotation == 0)
            {
                playerMovement.maxGravity = origionalGravity;
                playerMovement.gravityDirection = -player.transform.up;
                defaultMovementState.cancelBodyMovement = 1;
                rotate = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.transform.eulerAngles.z == 270)
            {
                rotate = true;
                fullRotation = -90;
                currentRotation = fullRotation;
            }
            else if (player.transform.eulerAngles.z == 90)
            {
                rotate = true;
                fullRotation = 90;
                currentRotation = fullRotation;
            }

            if (Mathf.Approximately((player.transform.eulerAngles.z), 330) && wallRunMoveState.isWallRunning == true)
            {
                rotate = true;
                fullRotation = -90;
                currentRotation = fullRotation;
                tilted = true;
            }
            else if (Mathf.Approximately((player.transform.eulerAngles.z), 30) && wallRunMoveState.isWallRunning == true)
            {
                tilted = true;
                rotate = true;
                fullRotation = 90;
                currentRotation = fullRotation;
            }
        }
    }

    void RotateAroundY(ref float amountToRotate)
    {
        float rotationToPreform;
        if (tilted)
        {
            rotationToPreform = fullRotation * (Time.deltaTime / secondsToRotateTilted);
        }
        else
        {
            rotationToPreform = fullRotation * (Time.deltaTime / secondsToRotateWall);
        }
        if (amountToRotate < 0)
        {
            if (amountToRotate - rotationToPreform > 0)
            {
                rotationToPreform = amountToRotate;
                amountToRotate = 0;
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + (rotationToPreform), player.transform.rotation.eulerAngles.z);
            }
            else
            {
                amountToRotate -= rotationToPreform;
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + (rotationToPreform), player.transform.rotation.eulerAngles.z);
            }
        }
        else if (amountToRotate > 0)
        {
            if (amountToRotate - rotationToPreform < 0)
            {
                rotationToPreform = amountToRotate;
                amountToRotate = 0;
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + (rotationToPreform), player.transform.rotation.eulerAngles.z);
            }
            else
            {
                amountToRotate -= rotationToPreform;
                player.transform.localRotation = Quaternion.Euler(
                   player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + (rotationToPreform), player.transform.rotation.eulerAngles.z);
            }
        }
    }
}
