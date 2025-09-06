using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class DefaultMovementState : MovementState
{
    // Variables

    [SerializeField] MovementState wallRunState;

    [SerializeField] int speed = 15;
    [SerializeField] int jumpForce = 15;
    [SerializeField] int jumpMax = 1;
    [SerializeField] int externalForceResistance = 2;
    [SerializeField] float externalForceThreshold = 1;
    [SerializeField] float wallRunDistance = 1.25f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float wallRunCooldown = 0.25f;

    public Vector3 playerVelocity;
    [HideInInspector] public Vector3 externalForceVelocity;
    int jumpCount;
    [HideInInspector] public float currentGravityVelocity;
    float wallRunCountdown;

    //Nicholas's aditional variable
    public float forwardMoveSpeed = 5f;
    [SerializeField] float forwardMoveSpeedMax = 30f;
    [SerializeField] float jumpDelay = 0.25f;
    [HideInInspector] public int cancelPlayerMovement = 1;
    [SerializeField] float groundedDistance = 1.1f;
    [HideInInspector] public bool isOnWall = false;
    [HideInInspector] public bool isOnRightWall = false;



    void Update()
    {
        forwardMoveSpeed = Mathf.Clamp(forwardMoveSpeed + forwardMoveSpeed * Time.deltaTime * 0.0001f, 0, forwardMoveSpeedMax);
    }

    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);
        playerMovement.RotateUprightWithGravity();
        externalForceVelocity = new Vector3(0, body.linearVelocity.y, 0);
        currentGravityVelocity = 0;
        jumpCount = jumpMax;
        wallRunCountdown = wallRunCooldown;
    }

    public override void OnUpdate(MoveInputStruct _input)
    {

        // calculate playerVelocity
            playerVelocity = (_input.moveInputVector.x * body.transform.right) * speed * cancelPlayerMovement;
            Vector3 forwardVelocity = body.transform.forward * forwardMoveSpeed;

        // handle gravity and jumping
        if (IsGrounded())
        { // on the ground
            jumpCount = 0;
            currentGravityVelocity = 0;
        }
        else
        { // off of the ground

            // add gravity/second to velocity
            if (!isOnWall)
            {
                currentGravityVelocity += playerMovement.gravityAcceleration * Time.deltaTime;
                if (currentGravityVelocity > playerMovement.maxGravity) currentGravityVelocity = playerMovement.maxGravity;
            }

            if (isOnWall)
            {
                jumpCount = 0;
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
        body.linearVelocity = forwardVelocity + playerVelocity + externalForceVelocity + playerMovement.gravityDirection * currentGravityVelocity;

        // check for change of state conditions
        StateCheck(_input);
    }

    public override void OnExit()
    {
        externalForceVelocity = Vector3.zero;
    }



    // Unique Functions

    bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, groundedDistance, groundLayers) && currentGravityVelocity >= 0)
            return true;
        else
            return false;
    }

    void Jump()
    {
        if (!isOnWall)
        {
            if (jumpCount < jumpMax)
            {
                jumpCount++;
                currentGravityVelocity = -jumpForce;
            }
        }
        else
        {
            if (jumpCount < 1)
            {
                isOnWall = false;
                int directionSwitch = (isOnRightWall) ? -1 : 1;
                jumpCount++;
                body.linearVelocity += (playerMovement.transform.right * directionSwitch * jumpForce);
                playerMovement.gravityDirection = -Vector3.up;
                playerMovement.ChangeToState(this);
            }
        }
    }

    void StateCheck(MoveInputStruct _input)
    {
        wallRunCountdown -= Time.deltaTime;

        if ((!IsGrounded() && wallRunCountdown <= 0))
        {
            RaycastHit hit;
            if ((Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward + body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers))
                || (Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers)))
            {
                float playerAngle = playerMovement.transform.rotation.z;
                if (playerAngle == 0 || playerAngle == 90 || playerAngle == 180 || playerAngle == 270)
                    playerMovement.ChangeToState(wallRunState);
            }
        }
        Debug.DrawRay(body.transform.position, Vector3.Normalize(body.transform.right + body.transform.forward - body.transform.up) * wallRunDistance, Color.blue);
        Debug.DrawRay(body.transform.position, Vector3.Normalize(-body.transform.right + body.transform.forward - body.transform.up) * wallRunDistance, Color.blue);
    }
}
