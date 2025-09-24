using UnityEngine;

public class DamageOnImpact : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject destroyObject;
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            if(destroyObject != null) Destroy(destroyObject);
        }
    }
}
