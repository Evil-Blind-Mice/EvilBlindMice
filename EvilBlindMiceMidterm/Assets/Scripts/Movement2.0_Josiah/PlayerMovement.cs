using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   public static PlayerMovement instance;
    [SerializeField] Rigidbody body;
    public RotationHandler rotHandle;
    public Transform gravityReference;

    public int gravityAcceleration = 50;
    public int maxGravity = 50;
    [HideInInspector] public float uprightRotation;
    [HideInInspector] public Intersection currentIntersection;

    [SerializeField] MovementState defaultMoveState;
    MovementState moveState;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeToState(defaultMoveState, true);
        SetGravityDirection(body.transform.forward, Vector3.up);
    }

    void Update()
    {
        moveState.OnUpdate(GetMoveInput());
    }

    MoveInputStruct GetMoveInput()
    {
        return new MoveInputStruct(
            Input.GetButtonDown("Sprint"),
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

    public void RotateUprightWithGravity(float _rotationSpeed = -1)
    {
        
        Quaternion lookRotation = Quaternion.LookRotation(gravityReference.forward, gravityReference.up);
        rotHandle.RotateSmooth(lookRotation, _rotationSpeed);
    }

    public void SetGravityDirection(Vector3 _forwardDirection, Vector3 _upDirection)
    {
        gravityReference.rotation = Quaternion.LookRotation(_forwardDirection, _upDirection);
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
