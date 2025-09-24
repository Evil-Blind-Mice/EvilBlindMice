using UnityEditor;
using UnityEngine;

public class HoverStateEnemy : CustomState
{
    Transform playerTransform;
    PlayerMovement playerScript;
    [SerializeField] Rigidbody body;
    [SerializeField] float moveSpeed;
    [SerializeField] float stoppingDistance;
    [SerializeField] RotationHandler rotHandle;
    [SerializeField] float avoidanceRadius;
    [SerializeField] LayerMask avoidLayers;
    [SerializeField] float duration;
    [SerializeField] CustomState nextState;

    float changeStateTimer;
    Vector3 targetVelocity;
    private void Start()
    {
        playerTransform = PlayerMovement.instance.transform;
        playerScript = PlayerMovement.instance;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        changeStateTimer = 0;
    }
    public override void OnUpdate()
    {
        // Reset Variables
        targetVelocity = Vector3.zero;

        // Player Tracking
        Vector3 directionToPlayer = ClampVectorExclude((playerTransform.position + playerScript.gravityReference.up) - body.position, playerScript.gravityReference.forward);
        targetVelocity = directionToPlayer.normalized * moveSpeed * (directionToPlayer.magnitude < 1 ? directionToPlayer.magnitude : 1);

        body.linearVelocity = targetVelocity;

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
