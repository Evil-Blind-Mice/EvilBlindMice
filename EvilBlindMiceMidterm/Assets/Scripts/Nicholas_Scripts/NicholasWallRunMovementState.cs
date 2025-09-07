using UnityEngine;

public class NicholasWallRunMovementState : NicholasMovementState
{
    // Variables

    [SerializeField] public NicholasDefaultMovementState defaultMovementState;

    [SerializeField] int jumpForce = 30;
    [SerializeField] float wallRunDistance = 2f;
    [SerializeField] int tiltDegree = 30;
    [SerializeField] float distanceFromGround = 1.2f;
    [SerializeField] LayerMask groundLayers;

    Vector3 wallNormal;
    int origionalMaxGravity;
    [HideInInspector] public bool isWallRunning = false;



    // Overridden Functions

    public override void NicholasOnEnter(NicholasPlayerMovement _playerMovement, Rigidbody _body)
    {
        base.NicholasOnEnter(_playerMovement, _body);

        // lean so that bottom of player is closer to wall
        RaycastHit hit;
        if(Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward + body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers))
        { // raycast that triggered was to the right of the player
            body.transform.localEulerAngles += new Vector3(0, 0,
                90 - Vector3.Angle(hit.normal, playerMovement.gravityDirection) + tiltDegree);
            origionalMaxGravity = playerMovement.maxGravity;
            playerMovement.maxGravity = 0;
            body.linearVelocity = Vector3.zero;
            body.linearVelocity = body.transform.forward * playerMovement.GetComponent<NicholasDefaultMovementState>().forwardMoveSpeed;
            isWallRunning = true;
        }
        else
        { // raycast that triggered was to the left of the player
            Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers);
            body.transform.localEulerAngles -= new Vector3(0, 0, 
                90 - Vector3.Angle(hit.normal, playerMovement.gravityDirection) + tiltDegree);
            origionalMaxGravity = playerMovement.maxGravity;
            playerMovement.maxGravity = 0;
            body.linearVelocity = Vector3.zero;
            body.linearVelocity = body.transform.forward * playerMovement.GetComponent<NicholasDefaultMovementState>().forwardMoveSpeed;
            isWallRunning = true;
        }
        wallNormal = hit.normal;

        // get the wall's normal to figure out which direction the player should move
    }

    public override void NicholasOnUpdate(NicholasMoveInputStruct _input)
    {
        NicholasStateCheck(_input);
        defaultMovementState.forwardMoveSpeed += (defaultMovementState.forwardMoveSpeed * Time.deltaTime * 0.0001f);
        body.linearVelocity = body.transform.forward * defaultMovementState.forwardMoveSpeed;
    }

    public override void NicholasOnExit()
    {

    }



    // Unique Functions

    void NicholasStateCheck(NicholasMoveInputStruct _input)
    {
        // if the player jumps off of the wall
        if (_input.jumpPressedThisFrame)
        {
            playerMovement.gravityDirection = -Vector3.up;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.NicholasRotateUprightWithGravity();
            body.linearVelocity = Vector3.Normalize(wallNormal + Vector3.up) * jumpForce;
            playerMovement.NicholasChangeToState(defaultMovementState);
            return;
        }

        // if the player presses shift to change gravity
        if (_input.shiftPressed)
        {
            defaultMovementState.isOnWall = true;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.gravityDirection = -wallNormal;
            playerMovement.NicholasChangeToState(defaultMovementState);
            return;
        }

        //if the player is close to the ground
        if (body.transform.position.y < distanceFromGround)
        {
            playerMovement.gravityDirection = -Vector3.up;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.NicholasRotateUprightWithGravity();
            playerMovement.NicholasChangeToState(defaultMovementState);
        }



    }
}
