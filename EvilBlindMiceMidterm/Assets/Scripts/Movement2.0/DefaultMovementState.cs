using UnityEngine;

public class DefaultMovementState : MovementState
{
    [SerializeField] int defaultSpeed = 10;
    [SerializeField] int sprintSpeed = 15;
    [SerializeField] int jumpForce = 10;
    [SerializeField] int jumpMax = 1;
    [SerializeField] float groundedDistance = 1.1f;
    [SerializeField] LayerMask groundLayers;

    Vector3 playerVelocity;
    int speed;
    int jumpCount;
    float currentGravityVelocity;

    // Overridden Functions

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        playerVelocity = Vector3.zero;

        if (_input.sprintPressed) speed = sprintSpeed;
        else speed = defaultSpeed;

        Vector3 moveDirection = (_input.moveInput.x * body.transform.right) +
            (_input.moveInput.y * body.transform.forward);

        if (IsGrounded())
        {
            jumpCount = 0;
            currentGravityVelocity = 0;
        }
        else
        {
            currentGravityVelocity += playerMovement.gravity * Time.deltaTime;
        }

        // controller.Move(moveDirection * speed * Time.deltaTime);
        playerVelocity += (moveDirection * speed);

        if (_input.jumpPressedThisFrame) Jump();

        //controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity += playerMovement.gravityDirection * currentGravityVelocity;

        body.linearVelocity = playerVelocity;
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
}
