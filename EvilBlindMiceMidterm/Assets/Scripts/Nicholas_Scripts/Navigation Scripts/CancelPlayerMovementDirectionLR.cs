using UnityEngine;
using UnityEngine.EventSystems;
using static NESWDirections;

public class CancelPlayerMovementDirectionLR : MonoBehaviour
{
    GameObject player;
    NicholasPlayerMovement playerMovement;
    NicholasWallRunMovementState wallRunMoveState;
    NicholasDefaultMovementState defaultMovementState;
    [SerializeField] FacingDirection facingDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<NicholasPlayerMovement>();
        wallRunMoveState = player.GetComponent<NicholasWallRunMovementState>();
        defaultMovementState = player.GetComponent<NicholasDefaultMovementState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            defaultMovementState.isCancelingOneMoveDirection = true;
            if (facingDirection == FacingDirection.East)
            {
                defaultMovementState.directionToCancel = 1;
            }
            if (facingDirection == FacingDirection.West)
            {
                defaultMovementState.directionToCancel = -1;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            defaultMovementState.isCancelingOneMoveDirection = false;
            defaultMovementState.directionToCancel = 0;
        }
    }
}
