using System.Collections;
using UnityEngine;

public class LaserBeamStateEnemy : CustomState
{
    [SerializeField] float duration;
    [SerializeField] Transform laserTransform;
    [SerializeField] int laserLength = 1000;
    [SerializeField] int extensionSpeed;
    [SerializeField] Transform bodyTransform;
    [SerializeField] CustomState nextState;
    float changeStateTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        changeStateTimer = 0;
        StartCoroutine(ExtendLaser());
        laserTransform.gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        changeStateTimer += Time.deltaTime;
        if (changeStateTimer > duration) machine.ChangeToState(nextState);
    }

    public override void OnExit()
    {
        base.OnExit();
        laserTransform.gameObject.SetActive(true);
    }

    IEnumerator ExtendLaser()
    {
        laserTransform.localScale = Vector3.one;
        float timer = 0;

        while(laserTransform.localScale.z < laserLength)
        {
            timer += Time.deltaTime;
            laserTransform.localScale = new Vector3(0, 0, timer * extensionSpeed);
            yield return new WaitForEndOfFrame();
        }

        laserTransform.localScale = new Vector3(0, 0, timer * laserLength);
    }

}
