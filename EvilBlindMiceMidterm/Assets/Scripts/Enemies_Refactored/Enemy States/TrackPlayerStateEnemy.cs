using UnityEngine;

public class TrackPlayerStateEnemy : CustomState
{
    [SerializeField] GameObject pivotPoint;

    private void Start()
    {
        if (pivotPoint == null) pivotPoint = gameObject;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        pivotPoint.transform.rotation = Quaternion.LookRotation(GameManager.instance.player.transform.position - pivotPoint.transform.position);
    }
}
