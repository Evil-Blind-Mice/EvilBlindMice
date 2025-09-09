using UnityEngine;
using UnityEngine.EventSystems;
using static NESWDirections;

public class CancelPlayerMovementDirectionLR : MonoBehaviour
{
    GameObject player;
    NicholasPlayerMovement playerMovement;
    NicholasMovementState moveState;
    NicholasWallRunMovementState wallRunMoveState;
    NicholasDefaultMovementState defaultMovementState;
    [SerializeField] FacingDirection facingDirection; //relative to player
    bool hasBeenTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    private void Update()
    {
        if (hasBeenTriggered)
        {
            if (defaultMovementState.isCancelingOneMoveDirection == true && moveState != playerMovement.moveState)
            {
                    defaultMovementState.isCancelingOneMoveDirection = false;
                    defaultMovementState.directionToCancel = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<NicholasPlayerMovement>();
        wallRunMoveState = player.GetComponent<NicholasWallRunMovementState>();
        defaultMovementState = player.GetComponent<NicholasDefaultMovementState>();

        if (other.CompareTag("Player"))
        {
            if (defaultMovementState.isOnWall == false)
            {
                if (facingDirection == FacingDirection.East)
                {
                    defaultMovementState.isCancelingOneMoveDirection = true;
                    defaultMovementState.directionToCancel = 1;
                    moveState = playerMovement.moveState;
                    hasBeenTriggered = true;
                }
                if (facingDirection == FacingDirection.West)
                {
                    defaultMovementState.isCancelingOneMoveDirection = true;
                    defaultMovementState.directionToCancel = -1;
                    moveState = playerMovement.moveState;
                    hasBeenTriggered = true;
                }
            }
            if (defaultMovementState.isOnWall == true)
            {
                if (facingDirection == FacingDirection.North)
                {
                    defaultMovementState.isCancelingOneMoveDirection = true;
                    defaultMovementState.directionToCancel = 1;
                    moveState = playerMovement.moveState;
                    hasBeenTriggered = true;
                }
                if (facingDirection == FacingDirection.South)
                {
                    defaultMovementState.isCancelingOneMoveDirection = true;
                    defaultMovementState.directionToCancel = -1;
                    moveState = playerMovement.moveState;
                    hasBeenTriggered = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            defaultMovementState.isCancelingOneMoveDirection = false;
            defaultMovementState.directionToCancel = 0;
            hasBeenTriggered = false;
        }
    }
}
