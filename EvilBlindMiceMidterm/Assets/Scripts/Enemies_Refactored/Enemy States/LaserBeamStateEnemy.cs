using System.Collections;
using UnityEngine;

public class LaserBeamStateEnemy : CustomState
{
    [Header("Laser")]
    [SerializeField] Transform laserTransform;
    [SerializeField] int laserLength = 1000;
    [SerializeField] int extensionSpeed;

    [Header("Other")]
    [SerializeField] float duration;
    [SerializeField] float pauseBeforeFiring;
    [SerializeField] Transform bodyTransform;
    [SerializeField] CustomState nextState;

    [Header("Materials (optional)")]
    [SerializeField] bool swapMaterial;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material alternateMaterial;

    Material defaultMaterial;
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

        if (swapMaterial)
        {
            defaultMaterial = renderer.material;
            renderer.material = alternateMaterial;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        changeStateTimer += Time.deltaTime;
        if (changeStateTimer > duration + pauseBeforeFiring) machine.ChangeToState(nextState);
    }

    public override void OnExit()
    {
        changeStateTimer = 0;
        base.OnExit();
        StopAllCoroutines();
        laserTransform.localScale = initialScale;
        laserTransform.gameObject.SetActive(false);
        if (swapMaterial) renderer.material = defaultMaterial;
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
