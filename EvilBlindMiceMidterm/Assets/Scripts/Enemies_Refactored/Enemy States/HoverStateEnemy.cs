using UnityEngine;

public class HoverStateEnemy : CustomState
{
    Transform playerTransform;
    PlayerMovement playerScript;
    [SerializeField] Rigidbody body;
    [SerializeField] float moveSpeed;
    [SerializeField] float stoppingDistance;
    [SerializeField] RotationHandler rotHandle;
    private void Start()
    {
        playerTransform = PlayerMovement.instance.transform;
        playerScript = PlayerMovement.instance;
    }
    public override void OnUpdate()
    {
        Vector3 directionToPlayer = playerTransform.position - body.transform.position - body.transform.forward * Vector3.Dot(playerTransform.position - body.position, body.transform.forward);
        if(directionToPlayer.magnitude > stoppingDistance)
            body.linearVelocity = directionToPlayer * directionToPlayer.magnitude * moveSpeed;

        rotHandle.RotateSmooth(Quaternion.LookRotation(-playerTransform.forward, playerTransform.up));
    }
}
