using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class LaserProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;

    int damage;
    float lifeSeconds;
    WeaponStats weaponForFx;
    GameObject owner;

    public void Init(Vector3 _direction, float _speed, float _lifeSeconds, int _damage, GameObject _owner, WeaponStats _weaponForFx = null)
    {
        damage = _damage;
        lifeSeconds = Mathf.Max(0.05f, _lifeSeconds);
        weaponForFx = _weaponForFx;
        owner = _owner;

        if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.linearVelocity = _direction.normalized * _speed;

        // ignore hitting the shooter
        Collider myCollider = GetComponent<Collider>();
        if (_owner)
        {
            Collider[] ownerColliders = _owner.GetComponentsInChildren<Collider>();
            foreach (Collider collider in ownerColliders)
                Physics.IgnoreCollision(myCollider, collider, true);
        }

        Destroy(gameObject, lifeSeconds);
    }

    void OnCollisionEnter(Collision _collision)
    {
        IDamage hittable = _collision.collider.GetComponent<IDamage>();
        if (hittable != null) hittable.TakeDamage(damage);

        if (weaponForFx && weaponForFx.hitEffect)
        {
            ContactPoint contactPoint = _collision.GetContact(0);
            Instantiate(weaponForFx.hitEffect, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        }

        TrailRenderer trailRenderer = GetComponentInChildren<TrailRenderer>();
        if (trailRenderer)
        {
            trailRenderer.transform.SetParent(null, true);
            trailRenderer.emitting = false;
            Destroy(trailRenderer.gameObject, trailRenderer.time);
        }

        Destroy(gameObject);
    }
}

