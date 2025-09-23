using UnityEngine;

[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{
    public GameObject weaponModel;
    public bool isAutomatic;
    [Range(1, 5000)] public int weaponFiringDamage;
    [Range(0.1f, 5000)] public float weaponFireRate;
    [Range(1, 5000)] public int weaponFiringDistance;
    public int weaponCurrentAmmo;
    [Range(1, 5000)] public int weaponMaxAmmo;

    public ParticleSystem hitEffect;
    public AudioClip[] shootingSound;
    [Range(0,1)] public float shootingSoundVolume;
}