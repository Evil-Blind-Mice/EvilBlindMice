using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, DOT, homing }
    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rigidBody;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == DamageType.moving || type == DamageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if(type == DamageType.moving)
            {
                rigidBody.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(type == DamageType.homing)
        {
            rigidBody.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && (type == DamageType.moving || type == DamageType.stationary || type == DamageType.homing))
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == DamageType.homing || type == DamageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == DamageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(DamageOther(dmg));
            }
        }
    }

    IEnumerator DamageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
