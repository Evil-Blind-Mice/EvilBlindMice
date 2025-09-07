using NUnit.Framework.Api;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody body;

    public int gravityAcceleration = 50;
    public int maxGravity = 50;
    [HideInInspector] public Vector3 gravityDirection;
    [HideInInspector] public float uprightRotation;

    [SerializeField] DefaultMovementState defaultMoveState;
    MovementState moveState;

    private void Start()
    {
        ChangeToState(defaultMoveState, true);
        gravityDirection = -Vector3.up;
    }

    void Update()
    {
        moveState.OnUpdate(GetMoveInput());
    }

    MoveInputStruct GetMoveInput()
    {
        return new MoveInputStruct(
            Input.GetButton("Sprint"),
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            Input.GetButtonDown("Jump")
            );
    }

    public void ChangeToState(MovementState _newState, bool _initializing = false)
    {
        if(!_initializing) moveState.OnExit();
        moveState = _newState;
        moveState.OnEnter(this, body);
    }

    public void RotateUprightWithGravity()
    {
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(body.transform.forward, gravityDirection), -gravityDirection);
        body.transform.rotation = lookRotation;
    }
}

public struct MoveInputStruct
{
    public bool shiftPressed;
    public Vector2 moveInputVector;
    public bool jumpPressedThisFrame;

    public MoveInputStruct(bool _sprintPressed, Vector2 _moveInputVector, bool _jumpPressedThisFrame)
    {
        shiftPressed = _sprintPressed;
        moveInputVector = _moveInputVector;
        jumpPressedThisFrame = _jumpPressedThisFrame;
    }
}
