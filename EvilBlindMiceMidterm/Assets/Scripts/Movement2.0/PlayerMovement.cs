using NUnit.Framework.Api;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody body;

    public int gravity;
    [SerializeField] int tiltSpeed;
    [SerializeField] int wallRunDistance;
    [HideInInspector] public Vector3 gravityDirection;
    [HideInInspector] public float uprightRotation;

    [SerializeField] MovementState defaultMoveState;
    MovementState moveState;

    private void Start()
    {
        ChangeToState(defaultMoveState, true);
        gravityDirection = -transform.up;
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
}

public struct MoveInputStruct
{
    public bool sprintPressed;
    public Vector2 moveInputVector;
    public bool jumpPressedThisFrame;

    public MoveInputStruct(bool _sprintPressed, Vector2 _moveInputVector, bool _jumpPressedThisFrame)
    {
        sprintPressed = _sprintPressed;
        moveInputVector = _moveInputVector;
        jumpPressedThisFrame = _jumpPressedThisFrame;
    }
}
