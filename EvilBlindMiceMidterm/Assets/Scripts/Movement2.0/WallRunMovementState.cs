using UnityEngine;

public class WallRunMovementState : MovementState
{
    // Variables

    [SerializeField] public MovementState defaultMovementState;

    [SerializeField] int jumpForce = 30;
    [SerializeField] float wallRunDistance = 2f;
    [SerializeField] int tiltDegree = 30;
    [SerializeField] float distanceFromGround = 1.2f;
    [SerializeField] LayerMask groundLayers;

    Vector3 wallNormal;
    int origionalMaxGravity;



    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);

        // lean so that bottom of player is closer to wall
        RaycastHit hit;
        if(Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward + body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers))
        { // raycast that triggered was to the right of the player
            body.transform.localEulerAngles += new Vector3(0, 0,
                90 - Vector3.Angle(hit.normal, playerMovement.gravityDirection) + tiltDegree);
            origionalMaxGravity = playerMovement.maxGravity;
            playerMovement.maxGravity = 0;
            body.linearVelocity = Vector3.zero;
            body.linearVelocity = body.transform.forward * playerMovement.GetComponent<DefaultMovementState>().forwardMoveSpeed;
        }
        else
        { // raycast that triggered was to the left of the player
            Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers);
            body.transform.localEulerAngles -= new Vector3(0, 0, 
                90 - Vector3.Angle(hit.normal, playerMovement.gravityDirection) + tiltDegree);
            origionalMaxGravity = playerMovement.maxGravity;
            playerMovement.maxGravity = 0;
            body.linearVelocity = Vector3.zero;
            body.linearVelocity = body.transform.forward * playerMovement.GetComponent<DefaultMovementState>().forwardMoveSpeed;
        }
        wallNormal = hit.normal;

        // get the wall's normal to figure out which direction the player should move
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        StateCheck(_input);
        playerMovement.GetComponent<DefaultMovementState>().forwardMoveSpeed += (playerMovement.GetComponent<DefaultMovementState>().forwardMoveSpeed * Time.deltaTime * 0.0001f);
        body.linearVelocity = body.transform.forward * playerMovement.GetComponent<DefaultMovementState>().forwardMoveSpeed;
    }

    public override void OnExit()
    {

    }



    // Unique Functions

    void StateCheck(MoveInputStruct _input)
    {
        // if the player jumps off of the wall
        if (_input.jumpPressedThisFrame)
        {
            playerMovement.gravityDirection = -Vector3.up;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.RotateUprightWithGravity();
            body.linearVelocity = Vector3.Normalize(wallNormal + Vector3.up) * jumpForce;
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

        // if the player presses shift to change gravity
        if (_input.shiftPressed)
        {
            playerMovement.GetComponent<DefaultMovementState>().isOnWall = true;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.gravityDirection = -wallNormal;
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

        //Marking for removal
        // if the player is no longer close to the wall
        //if (wallIsRight)
        //{
        //    if(!Physics.Raycast(body.transform.position, -wallNormal, wallRunDistance, groundLayers))
        //    {
        //        playerMovement.maxGravity = origionalMaxGravity;
        //        playerMovement.ChangeToState(defaultMovementState);
        //        return;
        //    }
        //    Debug.DrawRay(body.transform.position, -wallNormal * wallRunDistance, Color.blue);
        //}

        //if the player is close to the ground
        if (body.transform.position.y < distanceFromGround)
        {
            playerMovement.gravityDirection = -Vector3.up;
            playerMovement.maxGravity = origionalMaxGravity;
            playerMovement.RotateUprightWithGravity();
            playerMovement.ChangeToState(defaultMovementState);
        }



    }
}
