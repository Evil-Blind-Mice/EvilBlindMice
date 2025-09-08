using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody body;

    public int gravityAcceleration = 50;
    public int maxGravity = 50;
    public int rotationSpeed;
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
            Input.GetAxis("Horizontal"),
            Input.GetButtonDown("Jump")
            );
    }

    public void ChangeToState(MovementState _newState, bool _initializing = false)
    {
        if (!_initializing) moveState.OnExit();
        moveState = _newState;
        moveState.OnEnter(this, body);
    }

    public void RotateUprightWithGravity()
    {
        Quaternion lookRotation = Quaternion.LookRotation(body.transform.forward, -gravityDirection);
        StartCoroutine(LerpRotation(lookRotation));
    }

    public IEnumerator LerpRotation(Quaternion _lookRotation)
    {
        while(body.transform.rotation != _lookRotation)
        {
            body.transform.rotation = Quaternion.Lerp(body.transform.rotation, _lookRotation, rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}

public struct MoveInputStruct
{
    public bool shiftPressed;
    public float leftRightAxis;
    public bool jumpPressedThisFrame;

    public MoveInputStruct(bool _sprintPressed, float _leftRightAxis, bool _jumpPressedThisFrame)
    {
        shiftPressed = _sprintPressed;
        leftRightAxis = _leftRightAxis;
        jumpPressedThisFrame = _jumpPressedThisFrame;
    }
}
