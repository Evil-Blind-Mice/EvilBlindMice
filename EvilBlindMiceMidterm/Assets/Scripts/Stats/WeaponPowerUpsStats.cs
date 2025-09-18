using UnityEngine;

public class WeaponStats : ScriptableObject
{
    public GameObject weaponModel;
    [Range(1, 5000)] public int weaponFiringDamage;
    [Range(1, 5000)] public float weaponFireRate;
    [Range(1, 5000)] public int weaponFiringDistance;
    public int currentWeaponAmmo;
    [Range(1, 5000)] public int maxWeaponAmmo;

    public ParticleSystem hitEffect;
    public AudioClip[] shootingSound;
    [Range(0,1)] public float shootingSoundVolume;
}

public class PowerUpStats : ScriptableObject
{

}
