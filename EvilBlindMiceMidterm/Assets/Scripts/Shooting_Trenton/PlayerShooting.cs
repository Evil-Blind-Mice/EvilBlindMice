using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] int shootDamage;
    [SerializeField] float fireRate;
    [SerializeField] float shootDistance;

    float shootTimer;

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && shootTimer >= fireRate)
            Shoot();
    }

    public void Shoot()
    {
        shootTimer = 0f;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreLayers))
        {
            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null)
                damage.TakeDamage(shootDamage);
        }
    }
}
