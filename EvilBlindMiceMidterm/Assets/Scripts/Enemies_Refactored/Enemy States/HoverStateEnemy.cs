using UnityEditor;
using UnityEngine;

public class HoverStateEnemy : CustomState
{
    Transform playerTransform;
    PlayerMovement playerScript;
    [SerializeField] Rigidbody body;
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float decelerationDistance;
    [SerializeField] RotationHandler rotHandle;
    [SerializeField] float hoverHeight;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 durationRange;
    [SerializeField] CustomState nextState;

    float changeStateTimer;
    float duration;
    Vector3 targetVelocity;

    private void Start()
    {
        playerTransform = PlayerMovement.instance.transform;
        playerScript = PlayerMovement.instance;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        duration = Random.Range(durationRange.x, durationRange.y);
        changeStateTimer = 0;
    }
    public override void OnUpdate()
    {
        // Reset Variables
        targetVelocity = Vector3.zero;
        float targetOffset = 0;

        // Hover Height
        RaycastHit hit;
        Physics.Raycast(body.position, -playerScript.gravityReference.up, out hit, hoverHeight, groundLayer);
        if (hit.collider != null) targetOffset = hoverHeight - Vector3.Distance(hit.point, body.position);

        // Player Tracking
        Vector3 directionToTarget = ClampVectorExclude((playerTransform.position + (playerScript.gravityReference.up * targetOffset)) - body.position, playerScript.gravityReference.forward);
        targetVelocity = directionToTarget.normalized * moveSpeed * (directionToTarget.magnitude < decelerationDistance ? directionToTarget.magnitude/(decelerationDistance*deceleration) : 1);

        Vector3 velocityDiff = targetVelocity - body.linearVelocity;
        body.linearVelocity += velocityDiff * Time.deltaTime * acceleration;

        if(body.transform.up != playerScript.gravityReference.up && rotHandle.isUpright)
            rotHandle.RotateSmooth(Quaternion.LookRotation(-playerTransform.forward, playerScript.gravityReference.up));

        changeStateTimer += Time.deltaTime;
        if (changeStateTimer > duration) machine.ChangeToState(nextState);
    }

    Vector3 ClampVectorExclude(Vector3 _inputVec, Vector3 _direction)
    {
        return _inputVec - _direction * Vector3.Dot(_inputVec, _direction);
    }

    public override void OnExit()
    {
        base.OnExit();
        body.linearVelocity = Vector3.zero;
    }
}
