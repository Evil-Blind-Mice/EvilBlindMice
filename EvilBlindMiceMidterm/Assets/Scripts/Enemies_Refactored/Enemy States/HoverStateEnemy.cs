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

    Vector3 targetVelocity;
    private void Start()
    {
        playerTransform = PlayerMovement.instance.transform;
        playerScript = PlayerMovement.instance;
    }
    public override void OnUpdate()
    {
        // Reset Variables
        targetVelocity = Vector3.zero;

        // Player Tracking
        Vector3 directionToPlayer = ClampVectorExclude((playerTransform.position + playerScript.gravityReference.up) - body.position, playerScript.gravityReference.forward);
        targetVelocity = directionToPlayer.normalized * moveSpeed * (directionToPlayer.magnitude < 1 ? directionToPlayer.magnitude : directionToPlayer.magnitude * 2);
        
        body.linearVelocity = targetVelocity;

        if(body.transform.up != playerScript.gravityReference.up && rotHandle.isUpright)
            rotHandle.RotateSmooth(Quaternion.LookRotation(-playerTransform.forward, playerScript.gravityReference.up));
    }

    Vector3 ClampVectorExclude(Vector3 _inputVec, Vector3 _direction)
    {
        return _inputVec - _direction * Vector3.Dot(_inputVec, _direction);
    }
}
