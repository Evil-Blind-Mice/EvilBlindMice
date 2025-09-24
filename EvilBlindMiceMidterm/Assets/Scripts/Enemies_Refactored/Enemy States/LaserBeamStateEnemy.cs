using System.Collections;
using UnityEngine;

public class LaserBeamStateEnemy : CustomState
{
    [SerializeField] float duration;
    [SerializeField] float pauseBeforeFiring;
    [SerializeField] Transform laserTransform;
    [SerializeField] int laserLength = 1000;
    [SerializeField] int extensionSpeed;
    [SerializeField] Transform bodyTransform;
    [SerializeField] CustomState nextState;
    float changeStateTimer;
    Vector3 initialScale;

    private void Start()
    {
        initialScale = laserTransform.localScale;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        changeStateTimer = 0;
        laserTransform.localScale = initialScale;
        StartCoroutine(ExtendLaser());
        laserTransform.gameObject.SetActive(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        changeStateTimer += Time.deltaTime;
        if (changeStateTimer > duration + pauseBeforeFiring) machine.ChangeToState(nextState);
    }

    public override void OnExit()
    {
        base.OnExit();
        laserTransform.localScale = initialScale;
        laserTransform.gameObject.SetActive(false);
    }


    IEnumerator ExtendLaser()
    {
        yield return new WaitForSeconds(pauseBeforeFiring);
        float timer = 0;

        while(laserTransform.localScale.z < laserLength)
        {
            timer += Time.deltaTime;
            laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, timer * extensionSpeed);
            yield return new WaitForEndOfFrame();
        }

        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, timer * laserLength);
    }

}
