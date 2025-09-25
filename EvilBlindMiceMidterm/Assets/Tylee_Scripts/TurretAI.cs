using UnityEngine;

public class TurretAI : EnemyAI
{
    [SerializeField] GameObject TurretHead;

    private void Awake()
    {

    }

    void Update()
    {
        base.EnemyShield();

        base.shootTimer += Time.deltaTime;

        if (base.playerInTrigger && base.CanSeePlayer())
        {
            RotateTurrHead();
        }
    }

    void RotateTurrHead()
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - TurretHead.transform.position;

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        TurretHead.transform.rotation = Quaternion.Lerp(TurretHead.transform.rotation, rot, Time.deltaTime * base.faceTargetSpeed);
    }
}