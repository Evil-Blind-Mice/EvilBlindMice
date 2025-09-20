using System;
using System.Linq;
using TMPro;
using Unity.Burst;
using UnityEngine;

public class DefaultMovementState : MovementState, IDebug
{
    // Variables

    [SerializeField] MovementState dashState;
    [SerializeField] MovementState wallRunState;
    [SerializeField] float externalForceResistance = 0.5f;
    [SerializeField] float externalForceThreshold = 1;
    [SerializeField] float groundedDistance = 0.35f;
    [SerializeField] float wallRunDistance = 0.6f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float wallRunCooldown = 0.25f;
    [SerializeField] float wallRunCastOffset = 0.5f;


    Vector3 leftRightVelocity;
    Vector3 externalForceVelocity;
    int jumpCount;
    float currentGravityVelocity;
    float wallRunCountdown;
    float distanceToGround;
    float intersectionSpeed;
    float baseSpeed;

    // variables stored for debug
    GameObject currentWall;
    float currentWallAngle;
    string floorName;
    float rotationSpeedEquation;
   


    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);
        playerMovement.RotateUprightWithGravity();
        externalForceVelocity = body.linearVelocity;
        currentGravityVelocity = 0;
        if(IsGrounded()) jumpCount = 1;
        wallRunCountdown = wallRunCooldown;
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        base.OnUpdate(_input);

        float baseSpeed = (playerMovement.currentIntersection == null) ? PlayerStats.instance.GetSpeed() : Mathf.Clamp(PlayerStats.instance.GetSpeed(), intersectionSpeed, PlayerStats.instance.GetSpeed());

        if (playerMovement.currentIntersection != null)
        {
            externalForceVelocity = Vector3.zero;
        }

        // calculate playerVelocity
        leftRightVelocity = _input.leftRightAxis * body.transform.right * baseSpeed;


        // handle gravity and jumping
        if (IsGrounded())
        { // on the ground

            if (currentGravityVelocity >= 0)
            {
                jumpCount = 0;

                if(distanceToGround <= groundedDistance)
                    currentGravityVelocity = Vector3.Dot(externalForceVelocity, playerMovement.gravityReference.up);
            }

            // handle jumping
            if (_input.jumpPressedThisFrame) Jump();

        }
        else
        { // off of the ground

            // trigger dash
            if (_input.jumpPressedThisFrame && playerMovement.isUpright)
            {
                if (playerMovement.currentIntersection)
                {
                    if (!playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up))
                    {
                        Dash();

                    }
                }
                else Dash();

            }

                // add gravity/second to velocity
                currentGravityVelocity += playerMovement.gravityAcceleration * Time.deltaTime;
            if (currentGravityVelocity > playerMovement.maxGravity) currentGravityVelocity = playerMovement.maxGravity;

            // if the player reorients mid-air, the up direction is inverted
            if (_input.shiftPressed && playerMovement.isUpright)
            {
                playerMovement.SetGravityDirection(playerMovement.gravityReference.forward, -playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity(720);
            }
        }


        // handle external forces

        // The force of Air Resistance: F = -cv^2. 
        Vector3 resistance = new Vector3(
            Mathf.Clamp((Time.deltaTime * (externalForceResistance)), 0, Mathf.Abs(externalForceVelocity.x)),
            Mathf.Clamp((Time.deltaTime * (externalForceResistance)), 0, Mathf.Abs(externalForceVelocity.y)),
            Mathf.Clamp((Time.deltaTime * (externalForceResistance)), 0, Mathf.Abs(externalForceVelocity.z)));

        externalForceVelocity = new Vector3(
            externalForceVelocity.x + (externalForceVelocity.x > 0 ? -resistance.x : resistance.x),
            externalForceVelocity.y + (externalForceVelocity.y > 0 ? -resistance.y : resistance.y),
            externalForceVelocity.z + (externalForceVelocity.z > 0 ? -resistance.z : resistance.z));

        // if the external force is small enough, round to zero
        if (Mathf.Abs(externalForceVelocity.x) < externalForceThreshold) externalForceVelocity.x = 0;
        if (Mathf.Abs(externalForceVelocity.y) < externalForceThreshold) externalForceVelocity.y = 0;
        if (Mathf.Abs(externalForceVelocity.z) < externalForceThreshold) externalForceVelocity.z = 0;


        // apply all forces (playerVelocity, external forces, and gravity)
        body.linearVelocity = 
            leftRightVelocity // velocity determined by player input
            + externalForceVelocity // velocity from previous states or knockback
            - playerMovement.gravityReference.up * currentGravityVelocity
            + transform.forward * baseSpeed; // constant forward velocity

        // check for change of state conditions
        StateCheck(_input);
    }

    public override void OnInsideIntersection()
    {

        if (playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up) || playerMovement.currentIntersection == null) return;
        
        if (playerMovement.currentIntersection.IsDirectionAvailable(playerMovement.gravityReference.right))
        {
            if (Input.GetButtonDown("ChangeDirectionRight"))
            {
                playerMovement.SetGravityDirection(playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity();
                playerMovement.currentIntersection = null;
                ShowPrompts(false, false);
                return;
            }
        }
        if (playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.right))
        {
            if (Input.GetButtonDown("ChangeDirectionLeft"))
            {
                playerMovement.SetGravityDirection(-playerMovement.gravityReference.right, playerMovement.gravityReference.up);
                playerMovement.RotateUprightWithGravity();
                playerMovement.currentIntersection = null;
                ShowPrompts(false, false);
                return;
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

        float speedModifier = 7f;

        if (playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up))
        {
            currentGravityVelocity = 0;
            intersectionSpeed = (float)(4 * 0.785f * Mathf.Sqrt(distanceToGround) * speedModifier);
            body.linearVelocity = transform.forward * Mathf.Clamp(PlayerStats.instance.GetSpeed(), intersectionSpeed, PlayerStats.instance.GetSpeed());
            playerMovement.SetGravityDirection(-playerMovement.gravityReference.up, playerMovement.gravityReference.forward);
            float arcSegmentLength = (2f * MathF.PI * Mathf.Clamp(distanceToGround, 2.5f, 100)) / 4;
            playerMovement.RotateUprightWithGravity(90f/(arcSegmentLength/ Mathf.Clamp(PlayerStats.instance.GetSpeed(), intersectionSpeed, PlayerStats.instance.GetSpeed())));
            rotationSpeedEquation = 90f / (arcSegmentLength / PlayerStats.instance.GetSpeed());
            playerMovement.currentIntersection = null;
        }else 
        {
            ShowPrompts(
                playerMovement.currentIntersection.IsDirectionAvailable(playerMovement.gravityReference.right),
                playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.right));
        }

    }

    public override void OnIntersectionExit(Intersection _intersection, Vector3 _exitPoint)
    {
        base.OnIntersectionExit(_intersection, _exitPoint);

        ShowPrompts(false, false);

        Vector3 exitDirection = (body.transform.position - _exitPoint).normalized;

        intersectionSpeed = 0;
        
        if (Vector3.Angle(exitDirection, -playerMovement.gravityReference.up) < 5)
        { // player exited intersection going down
            playerMovement.SetGravityDirection(-playerMovement.gravityReference.up, playerMovement.gravityReference.forward);
            playerMovement.RotateUprightWithGravity();
        }
        else if (Vector3.Angle(exitDirection, playerMovement.gravityReference.right) < 5)
        { // player exited intersection to the right
            playerMovement.SetGravityDirection(playerMovement.gravityReference.right, playerMovement.gravityReference.up);
            playerMovement.RotateUprightWithGravity();
        }
        else if (Vector3.Angle(exitDirection, -playerMovement.gravityReference.right) < 5)
        { // player exited intersection to the left
            playerMovement.SetGravityDirection(-playerMovement.gravityReference.right, playerMovement.gravityReference.up);
            playerMovement.RotateUprightWithGravity();
        }
        else if (Vector3.Angle(exitDirection, playerMovement.gravityReference.up) < 5)
        { // player exited intersection... up?
            playerMovement.SetGravityDirection(playerMovement.gravityReference.up, -playerMovement.gravityReference.forward);
            playerMovement.RotateUprightWithGravity();
        }
        
    }

    void ShowPrompts(bool _left, bool _right)
    {
        GameManager.instance.IntersectionDirectionPromptLeft(_left);
        GameManager.instance.IntersectionDirectionPromptRight(_right);
    }


    // Unique Functions

    bool IsGrounded()
    {
        float checkBuffer = 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position - (transform.forward * 0.25f) + (playerMovement.gravityReference.up * 0.1f), -playerMovement.gravityReference.up, out hit, 100, groundLayers))
        {
            Debug.DrawRay(transform.position - (transform.forward * 0.25f) + (playerMovement.gravityReference.up * 0.1f), -playerMovement.gravityReference.up * distanceToGround, Color.blue);
            distanceToGround = Vector3.Distance(transform.position, hit.point);
            floorName = hit.collider.name;

            if (distanceToGround < groundedDistance + checkBuffer)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (jumpCount < PlayerStats.instance.GetJumpMax())
        {
            jumpCount++;
            currentGravityVelocity = -PlayerStats.instance.GetJumpForce();
        }
    }

    void Dash()
    {
        if(PlayerStats.instance.GetDashCount() > 0)
        {
            playerMovement.ChangeToState(dashState);
        }
    }

    void StateCheck(MoveInputStruct _input)
    {
        wallRunCountdown -= Time.deltaTime;

        if (!IsGrounded() && wallRunCountdown <= 0)
        {

            RaycastHit hit;
            if ((Physics.Raycast(body.transform.position + body.transform.up * wallRunCastOffset, body.transform.right, out hit, wallRunDistance, groundLayers) && _input.leftRightAxis > 0)
                || (Physics.Raycast(body.transform.position + body.transform.up * wallRunCastOffset, -body.transform.right, out hit, wallRunDistance, groundLayers) && _input.leftRightAxis < 0))
            {
                currentWall = hit.collider.gameObject;
                currentWallAngle = Vector3.Angle(hit.normal, playerMovement.gravityReference.up);
                if (currentWallAngle >= 55 && currentWallAngle <= 95)
                    playerMovement.ChangeToState(wallRunState);
            }
        }
        Debug.DrawRay(body.transform.position + body.transform.up * wallRunCastOffset, body.transform.right * wallRunDistance, Color.blue);
        Debug.DrawRay(body.transform.position + body.transform.up * wallRunCastOffset, - body.transform.right * wallRunDistance, Color.blue);
    }

    public DebugPacket GetDebugPacket()
    {
        return new DebugPacket
        (
            "Rotation Speed: " + rotationSpeedEquation,
            "Velocity: " + body.linearVelocity,
            "Is Grounded: " + IsGrounded(),
            currentWall != null ? "Wall Check hit: " + currentWall.name : "Wall Check hit: nothing",
            "Base Speed Applied " + baseSpeed,
            "Distance to ground: " + distanceToGround,
            "current gravity velocity: " + currentGravityVelocity,
            "External Force Velocity: " + externalForceVelocity
        );
    }
}
