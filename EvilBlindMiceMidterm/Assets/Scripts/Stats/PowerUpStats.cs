using UnityEngine;

public enum PowerUpType { Heal, SpeedBoost, Invincibility, TimeSlow, Dash, Trip }

[CreateAssetMenu]
public class PowerUpStats : ScriptableObject
{
    [Header("Type")]
    public PowerUpType type;
    public AudioClip[] sound;

    [Header("Heal")]
    public int healAmount;

    [Header("Speed Boost")]
    [Min(1)] public float speedMultiplier;
    public int speedDurationSeconds;

    [Header("Invincibility")]
    public int invincibilityDurationSeconds;

    [Header("Time Slow")]
    [Range(0.05f, 1)] public float slowScale;
    public int slowDurationSeconds;

    [Header("Dash Charge")]
    public int dashCharges;

    [Header("Trip/Obstacle")]
    public float tripSpeedDivider;
    public int tripDurationSeconds;
}
