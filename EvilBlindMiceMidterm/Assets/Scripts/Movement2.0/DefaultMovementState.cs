using TMPro;
using UnityEngine;

public class DefaultMovementState : MovementState
{
    // Variables

    [SerializeField] MovementState wallRunState;

    [SerializeField] int defaultSpeed = 10;
    [SerializeField] int sprintSpeed = 15;
    [SerializeField] int jumpForce = 10;
    [SerializeField] int jumpMax = 1;
    [SerializeField] float groundedDistance = 1.1f;
    [SerializeField] float wallRunDistance = 1f;
    [SerializeField] LayerMask groundLayers;

    Vector3 playerVelocity;
    int speed;
    int jumpCount;
    float currentGravityVelocity;



    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);
        currentGravityVelocity = 0;
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        playerVelocity = Vector3.zero;

        if (_input.sprintPressed) speed = sprintSpeed;
        else speed = defaultSpeed;

        Vector3 moveDirection = (_input.moveInputVector.x * body.transform.right) +
            (_input.moveInputVector.y * body.transform.forward);

        if (IsGrounded())
        {
            jumpCount = 0;
            currentGravityVelocity = 0;
        }
        else
        {
            currentGravityVelocity += playerMovement.gravity * Time.deltaTime;
        }

        playerVelocity += (moveDirection * speed);

        if (_input.jumpPressedThisFrame) Jump();

        playerVelocity += playerMovement.gravityDirection * currentGravityVelocity;

        body.linearVelocity = playerVelocity;

        StateCheck(_input);
    }

    public override void OnExit()
    {

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
        if(!IsGrounded())
        {
            if(Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward + body.transform.right), wallRunDistance, groundLayers)
                && _input.moveInputVector.x > 0)
            {
                playerMovement.ChangeToState(wallRunState);
            }
            else if (Physics.Raycast(body.transform.position, Vector3.Normalize(body.transform.forward - body.transform.right), wallRunDistance, groundLayers)
                && _input.moveInputVector.x < 0)
            {
                playerMovement.ChangeToState(wallRunState);
            }
        }
        Debug.DrawRay(body.transform.position, Vector3.Normalize(body.transform.right + body.transform.forward) * wallRunDistance, Color.blue);
        Debug.DrawRay(body.transform.position, Vector3.Normalize(-body.transform.right + body.transform.forward) * wallRunDistance, Color.blue);
    }
}
