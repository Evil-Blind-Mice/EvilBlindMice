using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    public Transform gravityReference;

    public int gravityAcceleration = 50;
    public int maxGravity = 50;
    public float rotationSpeed;
    //[HideInInspector] public Vector3 gravityDirection;
    [HideInInspector] public float uprightRotation;
    [HideInInspector] public bool isUpright;

    [SerializeField] MovementState defaultMoveState;
    MovementState moveState;

    Coroutine activeRotation;

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

    public void RotateUprightWithGravity()
    {
        
        Quaternion lookRotation = Quaternion.LookRotation(gravityReference.forward, gravityReference.up);
        RotateSmooth(lookRotation);
    }

    public void RotateSmooth(Quaternion _lookRotation) 
    {
        if (activeRotation != null) StopCoroutine(activeRotation);
        activeRotation = StartCoroutine(RotateSmoothCoroutine(_lookRotation));
    }

    IEnumerator RotateSmoothCoroutine(Quaternion _lookRotation)
    {
        isUpright = false;
        float timeCount = 0f;
        float slerpProgress = 0f;
        Quaternion startRotation = body.transform.rotation;
        float totalRotDegrees = Quaternion.Angle(_lookRotation, startRotation);

        while (slerpProgress < 1)
        {
            timeCount += Time.deltaTime;

            // rotate by rotationSpeed divided by the total number of degrees of rotation that will occur, multipled by time
            slerpProgress = timeCount * (rotationSpeed / (totalRotDegrees / 10));
            
            body.transform.rotation = Quaternion.Slerp(startRotation, _lookRotation, slerpProgress);

            yield return new WaitForEndOfFrame();
        }
        isUpright = true;
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
