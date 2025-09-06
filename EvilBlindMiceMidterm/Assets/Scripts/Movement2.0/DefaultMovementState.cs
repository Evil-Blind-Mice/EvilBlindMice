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
    [SerializeField] float groundedDistance = 1.1f;
    [SerializeField] float wallRunDistance = 1.25f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float wallRunCooldown = 0.25f;

    public Vector3 playerVelocity;
    Vector3 externalForceVelocity;
    int jumpCount;
    float currentGravityVelocity;
    float wallRunCountdown;

    //Nicholas's aditional variable
    [SerializeField] float forwardMoveSpeed;
    [HideInInspector] public int cancelPlayerMovement = 1;



    void Update()
    {
        forwardMoveSpeed = forwardMoveSpeed + forwardMoveSpeed * Time.deltaTime * 0.001f;
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
            currentGravityVelocity += playerMovement.gravityAcceleration * Time.deltaTime;
            if (currentGravityVelocity > playerMovement.maxGravity) currentGravityVelocity = playerMovement.maxGravity;

            // if the player reorients mid-air, they go back to world space up
            if (_input.shiftPressed)
            {
                playerMovement.gravityDirection = -Vector3.up;
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
        if (Physics.Raycast(transform.position, -transform.up, groundedDistance, groundLayers) && currentGravityVelocity >= 0)
            return true;
        else
            return false;
    }

    void Jump()
    {
        if (jumpCount < jumpMax)
        {
            jumpCount++;
            currentGravityVelocity = -jumpForce;
        }
    }

    void StateCheck(MoveInputStruct _input)
    {
        wallRunCountdown -= Time.deltaTime;

        if (!IsGrounded() && wallRunCountdown <= 0)
        {
            RaycastHit hit;
            if ((Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward + body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers) && _input.moveInputVector.x > 0)
                || (Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right - body.transform.up), out hit, wallRunDistance, groundLayers) && _input.moveInputVector.x < 0))
            {
                float normalAngle = Vector3.Angle(hit.normal, -playerMovement.gravityDirection);
                if (normalAngle >= 55 && normalAngle <= 95)
                    playerMovement.ChangeToState(wallRunState);
            }
        }
        Debug.DrawRay(body.transform.position, Vector3.Normalize(body.transform.right + body.transform.forward - body.transform.up) * wallRunDistance, Color.blue);
        Debug.DrawRay(body.transform.position, Vector3.Normalize(-body.transform.right + body.transform.forward - body.transform.up) * wallRunDistance, Color.blue);
    }
}
