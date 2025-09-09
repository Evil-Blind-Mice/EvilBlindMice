using UnityEditor;
using UnityEngine;

public class WallRunMovementState : MovementState
{
    // Variables

    [SerializeField] MovementState defaultMovementState;

    // [SerializeField] int normalSpeed = 15;
    [SerializeField] int intersectionSpeed = 3;
    // [SerializeField] int jumpForce = 30;
    [SerializeField] float wallRunDistance = 2f;
    [SerializeField] int tiltDegree = 30;
    [SerializeField] LayerMask groundLayers;

    Vector3 playerVelocity;
    [HideInInspector] public bool wallIsRight;
    Vector3 wallNormal;

    float speed;



    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {

        speed = PlayerStats.instance.GetSpeed();

        base.OnEnter(_playerMovement, _body);

        // lean so that bottom of player is closer to wall
        RaycastHit hit;
        if (Physics.Raycast(body.transform.position, body.transform.right, out hit, wallRunDistance, groundLayers))
        { // raycast that triggered was to the right of the player
            wallIsRight = true;
        }
        else if(Physics.Raycast(body.transform.position, -body.transform.right, out hit, wallRunDistance, groundLayers))
        { // raycast that triggered was to the left of the player
            wallIsRight = false;
        }
        else 
        {
            playerMovement.ChangeToState(defaultMovementState);
        }

        playerMovement.RotateSmooth(Quaternion.LookRotation(playerMovement.gravityReference.forward, Vector3.Slerp(playerMovement.gravityReference.up, hit.normal, tiltDegree / 90f)));
        wallNormal = hit.normal;
    }

    public override void OnExit()
    {
        base.OnExit();
        speed = PlayerStats.instance.GetSpeed();
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        playerVelocity = body.transform.forward * speed;

        body.linearVelocity = playerVelocity;

        StateCheck(_input);
    }

    public override void OnIntersectionEnter(Intersection _intersection)
    {
        base.OnIntersectionEnter(_intersection);

        Vector3 leanOutToward = body.transform.forward;


        if (wallIsRight)
        {
            if (_intersection.IsDirectionAvailable(playerMovement.gravityReference.right))
            {
                speed = intersectionSpeed;
                playerMovement.SetGravityDirection(playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateSmooth(Quaternion.LookRotation(playerMovement.gravityReference.forward, Vector3.Slerp(playerMovement.gravityReference.up, leanOutToward, tiltDegree / 90f)));
            }

        }
        else
        {
            if (_intersection.IsDirectionAvailable(-playerMovement.gravityReference.right))
            {
                speed = intersectionSpeed;
                playerMovement.SetGravityDirection(-playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateSmooth(Quaternion.LookRotation(playerMovement.gravityReference.forward, Vector3.Slerp(playerMovement.gravityReference.up, leanOutToward, tiltDegree / 90f)));
            }
            else
            {
                playerMovement.ChangeToState(defaultMovementState);
            }
        }

    }

    public override void OnIntersectionExit(Intersection _intersection)
    {
        base.OnIntersectionExit(_intersection);

        speed = PlayerStats.instance.GetSpeed();

        playerMovement.ChangeToState(this);
    }



    // Unique Functions

    void StateCheck(MoveInputStruct _input)
    {
        // if the player jumps off of the wall
        if (_input.jumpPressedThisFrame)
        {
            body.linearVelocity = Vector3.Normalize(body.transform.up * 2 + wallNormal) * PlayerStats.instance.GetJumpForce() * 2;
            TriggerDefaultState();
        }

        // if the player presses shift to change gravity
        if (_input.shiftPressed)
        {
            playerMovement.SetGravityDirection(transform.forward, wallNormal);
            TriggerDefaultState();
        }

        if (Physics.Raycast(body.transform.position, -playerMovement.gravityReference.up, wallRunDistance, groundLayers))
        {
            TriggerDefaultState();
        }

        if (!Physics.CheckSphere(body.transform.position, wallRunDistance, groundLayers))
        {
            TriggerDefaultState();
        }

    }

    void TriggerDefaultState()
    {
        playerMovement.ChangeToState(defaultMovementState);
        return;
    }
}
