using UnityEngine;

public class WallRunMovementState : MovementState
{
    // Variables

    [SerializeField] public MovementState defaultMovementState;

    [SerializeField] int speed = 15;
    [SerializeField] int jumpForce = 30;
    [SerializeField] float wallRunDistance = 2f;
    [SerializeField] int tiltDegree = 30;
    [SerializeField] LayerMask groundLayers;

    Vector3 playerVelocity;
    bool wallIsRight;
    Vector3 wallNormal;



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
            wallIsRight = true;
        }
        else
        { // raycast that triggered was to the left of the player
            Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers);
            body.transform.localEulerAngles -= new Vector3(0, 0, 
                90 - Vector3.Angle(hit.normal, playerMovement.gravityDirection) + tiltDegree);
            wallIsRight = false;
        }
        wallNormal = hit.normal;

        // get the wall's normal to figure out which direction the player should move
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        playerVelocity = Vector3.zero;

        playerVelocity += (body.transform.forward * speed);

        body.linearVelocity = playerVelocity;

        StateCheck(_input);
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
            body.linearVelocity = Vector3.Normalize(body.transform.up * 2 + wallNormal) * jumpForce;
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

        // if the player presses shift to change gravity
        if (_input.shiftPressed)
        {
            playerMovement.gravityDirection = -wallNormal;
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

        // if the player is no longer close to the wall
        if (wallIsRight)
        {
            if(!Physics.Raycast(body.transform.position, Vector3.Normalize(-body.transform.up + body.transform.right), wallRunDistance, groundLayers))
            {
                playerMovement.ChangeToState(defaultMovementState);
                return;
            }
            Debug.DrawRay(body.transform.position, Vector3.Normalize(body.transform.right - body.transform.up) * wallRunDistance, Color.blue);
        }
        else
        {
            if (!Physics.Raycast(body.transform.position, Vector3.Normalize(-body.transform.up - body.transform.right), wallRunDistance, groundLayers))
            {
                playerMovement.ChangeToState(defaultMovementState);
                return;
            }
            Debug.DrawRay(body.transform.position, Vector3.Normalize(-body.transform.right - body.transform.up) * wallRunDistance, Color.blue);
        }

        // if the player stops moving forward
        if (_input.moveInputVector.y <= 0)
        {
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

    }
}
