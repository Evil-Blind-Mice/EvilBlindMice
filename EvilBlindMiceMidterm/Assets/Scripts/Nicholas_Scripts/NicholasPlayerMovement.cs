using NUnit.Framework.Api;
using Unity.VisualScripting;
using UnityEngine;

public class NicholasPlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody body;

    public int gravityAcceleration = 50;
    public int maxGravity = 50;
    [HideInInspector] public Vector3 gravityDirection;
    [HideInInspector] public float uprightRotation;

    [SerializeField] NicholasDefaultMovementState defaultMoveState;
    [HideInInspector] public NicholasMovementState moveState;

    private void Start()
    {
        gravityDirection = -Vector3.up;
        NicholasChangeToState(defaultMoveState, true);
    }

    void Update()
    {
        moveState.NicholasOnUpdate(NicholasGetMoveInput());
    }

    NicholasMoveInputStruct NicholasGetMoveInput()
    {
        return new NicholasMoveInputStruct(
            Input.GetButton("Sprint"),
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            Input.GetButtonDown("Jump")
            );
    }

    public void NicholasChangeToState(NicholasMovementState _newState, bool _initializing = false)
    {
        if(!_initializing) moveState.NicholasOnExit();
        moveState = _newState;
        moveState.NicholasOnEnter(this, body);
    }

    public void NicholasRotateUprightWithGravity()
    {
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(body.transform.forward, gravityDirection), -gravityDirection);
        body.transform.rotation = lookRotation;
    }
}

public struct NicholasMoveInputStruct
{
    public bool shiftPressed;
    public Vector2 moveInputVector;
    public bool jumpPressedThisFrame;

    public NicholasMoveInputStruct(bool _sprintPressed, Vector2 _moveInputVector, bool _jumpPressedThisFrame)
    {
        shiftPressed = _sprintPressed;
        moveInputVector = _moveInputVector;
        jumpPressedThisFrame = _jumpPressedThisFrame;
    }
}
