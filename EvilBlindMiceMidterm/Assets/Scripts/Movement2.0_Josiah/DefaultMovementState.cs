using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DefaultMovementState : MovementState, IDebug
{
    // Variables

    [SerializeField] MovementState wallRunState;

    // [SerializeField] int speed = 15; replaced by player stats
    // [SerializeField] int jumpForce = 15; replaced by player stats
    // [SerializeField] int jumpMax = 1; replaced by player stats
    [SerializeField] int externalForceResistance = 2;
    [SerializeField] float externalForceThreshold = 1;
    [SerializeField] float groundedDistance = 0.1f;
    [SerializeField] float wallRunDistance = 0.6f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float wallRunCooldown = 0.25f;
    [SerializeField] Vector3 wallRunCastOffset = new Vector3(0, 0.5f, 0);

    Vector3 leftRightVelocity;
    Vector3 externalForceVelocity;
    int jumpCount;
    float currentGravityVelocity;
    float wallRunCountdown;


    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);
        playerMovement.RotateUprightWithGravity();
        externalForceVelocity = body.linearVelocity;
        currentGravityVelocity = 0;
        jumpCount = 1;
        wallRunCountdown = wallRunCooldown;
    }

    public override void OnUpdate(MoveInputStruct _input)
    {

        if (currentIntersection != null) OnInsideIntersection();

        // calculate playerVelocity
        leftRightVelocity = _input.leftRightAxis * body.transform.right * PlayerStats.instance.GetSpeed();


        // handle gravity and jumping
        if (IsGrounded())
        { // on the ground
            jumpCount = 0;
            currentGravityVelocity = 0;
        }
        else
        { // off of the ground

            // add gravity/second to velocity
            currentGravityVelocity += playerMovement.gravityAcceleration * Time.deltaTime;
            if (currentGravityVelocity > playerMovement.maxGravity) currentGravityVelocity = playerMovement.maxGravity;

            // if the player reorients mid-air, they go back to world space up
            if (_input.shiftPressed && playerMovement.isUpright)
            {
                playerMovement.SetGravityDirection(playerMovement.gravityReference.forward, -playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity();
            }
        }

        // handle jumping
        if (_input.jumpPressedThisFrame) Jump();


        // handle external forces

        // reduce external forces by the resistance percentage of their value
        externalForceVelocity = new Vector3(
            externalForceVelocity.x - (externalForceVelocity.x * Time.deltaTime * externalForceResistance),
            externalForceVelocity.y - (externalForceVelocity.y * Time.deltaTime * externalForceResistance),
            externalForceVelocity.z - (externalForceVelocity.z * Time.deltaTime * externalForceResistance)
            );

        // if the external force is small enough, round to zero
        if (Mathf.Abs(externalForceVelocity.x) < externalForceThreshold) externalForceVelocity.x = 0;
        if (Mathf.Abs(externalForceVelocity.y) < externalForceThreshold) externalForceVelocity.y = 0;
        if (Mathf.Abs(externalForceVelocity.z) < externalForceThreshold) externalForceVelocity.z = 0;


        // apply all forces (playerVelocity, external forces, and gravity)
        body.linearVelocity = 
            leftRightVelocity // velocity determined by player input
            + externalForceVelocity // velocity from previous states or knockback
            - playerMovement.gravityReference.up * currentGravityVelocity // velocity from jumping or gravity
            + transform.forward * PlayerStats.instance.GetSpeed(); // constant forward velocity

        // check for change of state conditions
        StateCheck(_input);
    }

    public override void OnInsideIntersection()
    {
        if (currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up)) return;

        if (currentIntersection.IsDirectionAvailable(playerMovement.gravityReference.right))
        {
            if(Input.GetButtonDown("ChangeDirectionRight"))
            {
                playerMovement.SetGravityDirection(playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity();
                currentIntersection = null;
            }
        }
        if (currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.right))
        {
            if (Input.GetButtonDown("ChangeDirectionLeft"))
            {
                playerMovement.SetGravityDirection(-playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity();
                currentIntersection = null;
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        externalForceVelocity = Vector3.zero;
    }

    public override void OnIntersectionEnter(Intersection _intersection)
    {
        base.OnIntersectionEnter(_intersection);

        if (_intersection.IsDirectionAvailable(-playerMovement.gravityReference.up))
        {
            playerMovement.SetGravityDirection(-playerMovement.gravityReference.up, playerMovement.gravityReference.forward);
            playerMovement.RotateUprightWithGravity();
        }

    }


    // Unique Functions

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, -transform.up * groundedDistance, Color.blue);
        if (Physics.Raycast(transform.position, -transform.up, groundedDistance, groundLayers) && currentGravityVelocity >= 0)
            return true;
        else
            return false;
    }

    void Jump()
    {
        if (jumpCount < PlayerStats.instance.GetJumpMax())
        {
            jumpCount++;
            currentGravityVelocity = -PlayerStats.instance.GetJumpForce();
        }
    }

    void StateCheck(MoveInputStruct _input)
    {
        wallRunCountdown -= Time.deltaTime;

        if (!IsGrounded() && wallRunCountdown <= 0)
        {
            RaycastHit hit;
            if ((Physics.Raycast(body.transform.position + wallRunCastOffset, body.transform.right, out hit, wallRunDistance, groundLayers) && _input.leftRightAxis > 0)
                || (Physics.Raycast(body.transform.position + wallRunCastOffset, -body.transform.right, out hit, wallRunDistance, groundLayers) && _input.leftRightAxis < 0))
            {
                float normalAngle = Vector3.Angle(hit.normal, playerMovement.gravityReference.up);
                if (normalAngle >= 55 && normalAngle <= 95)
                    playerMovement.ChangeToState(wallRunState);
            }
        }
        Debug.DrawRay(body.transform.position + wallRunCastOffset, body.transform.right * wallRunDistance, Color.blue);
        Debug.DrawRay(body.transform.position + wallRunCastOffset, - body.transform.right * wallRunDistance, Color.blue);
    }

    public DebugPacket GetDebugPacket()
    {
        return new DebugPacket
            (
                "Is Current State: " + isCurrentState,
                "Velocity: " + body.linearVelocity,
                "Is Grounded: " + IsGrounded()
            );
    }
}
