using UnityEngine;

public class DamageOnImpact : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject destroyObject;
    enum TargetType { player, enemy};
    [SerializeField] TargetType targetType = TargetType.player;
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            if (targetType == TargetType.player) 
            {
                if (!other.CompareTag("Player")) return;
            }

            if (targetType == TargetType.enemy) 
            {
                if (other.CompareTag("Player")) return;
            }

            dmg.TakeDamage(damage);
            if(destroyObject != null) Destroy(destroyObject);
        }
    }
}
