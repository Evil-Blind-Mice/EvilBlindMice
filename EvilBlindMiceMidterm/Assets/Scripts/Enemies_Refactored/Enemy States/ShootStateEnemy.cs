using UnityEngine;

public class ShootStateEnemy : TrackPlayerStateEnemy
{
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootRate;
    float shootTimer;

    public override void OnUpdate()
    {
        base.OnUpdate();

        ShootCheck();
    }

    void ShootCheck()
    {
        shootTimer += Time.deltaTime;
        if(shootTimer >= shootRate)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
    }
}
