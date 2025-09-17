using UnityEngine;

public class TrackPlayerStateEnemy : CustomState
{
    [SerializeField] GameObject pivotPoint;
    [SerializeField] Transform shootPos;

    private void Start()
    {
        if (pivotPoint == null) pivotPoint = gameObject;
        if (shootPos == null) shootPos = gameObject.transform;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        pivotPoint.transform.rotation = Quaternion.LookRotation(GameManager.instance.player.transform.position - pivotPoint.transform.position);
    }
}
