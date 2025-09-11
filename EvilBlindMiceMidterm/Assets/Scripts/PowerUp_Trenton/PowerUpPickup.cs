using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{
    public enum PickUpType
    {
        Heal, TimeSlow, Invincibility, SpeedBoost, Obstacle
    }

    [Header("Power Up Type")]
    [SerializeField] PickUpType type = PickUpType.Heal;

    [Header("Heal")]
    [SerializeField] int healAmount;

    [Header("Time Slow")]
    [SerializeField, Range(0.05f, 1f)] float slowScale;
    [SerializeField] int slowDurationSeconds;

    [Header("Invincibility")]
    [SerializeField] int invincibilityDurationSeconds;

    [Header("Speed Boost")]
    [SerializeField, Min(1)] float speedMultiplier;
    [SerializeField] int speedDurationSeconds;

    [Header("Obstacle")]
    [SerializeField] float speedDivider;
    [SerializeField] int speedDuration;

    static float activeSlowScale = 1f;
    static float slowRemainingSeconds = 0f;
    static Coroutine slowRoutine;
    static MonoBehaviour runner;

    void OnTriggerEnter(Collider _other)
    {
        if (_other.isTrigger) return;
        if (!_other.CompareTag("Player")) return;

        PlayerStats stats = PlayerStats.instance;
        if (stats == null) return;

        if (_other.transform.root != stats.transform.root) return;

        switch (type)
        {
            case PickUpType.Heal:
                ApplyHeal(stats, healAmount);
                break;

            case PickUpType.TimeSlow:
                ApplyTimeSlow(slowScale, slowDurationSeconds);
                break;

            case PickUpType.Invincibility:
                stats.RequestInvincibility(invincibilityDurationSeconds);
                break;

            case PickUpType.SpeedBoost:
                stats.RequestSpeedBoost(speedMultiplier, speedDurationSeconds);
                break;

            case PickUpType.Obstacle:
                stats.RequestTripState(speedDivider, speedDuration);
                break;
        }

        Destroy(gameObject);
    }

    // -*-*-*-*-*-*-*- Helper Methods -*-*-*-*-*-*-*-

    public static void ResetAllEffects()
    {
        if (slowRoutine != null)
        {
            MonoBehaviour runner = GetRunner();
            if (runner != null)
                runner.StopCoroutine(slowRoutine);
            slowRoutine = null;
        }

        activeSlowScale = 1f;
        slowRemainingSeconds = 0f;
        Time.timeScale = 1f;

        if (PlayerStats.instance != null)
            PlayerStats.instance.ResetAllPowerUpEffects();
    }

    static void ApplyHeal(PlayerStats _stats, int _amount)
    {
        if (_amount <= 0) return;

        int currentHealth = _stats.GetHealth();
        int maximumHealth = _stats.GetMaxHealth();
        int healthGain = Mathf.Clamp(_amount, 0, maximumHealth - currentHealth);

        if (healthGain != 0)
            _stats.AddHealth(healthGain);
    }

    static bool IsPaused()
    {
        return GameManager.instance != null && GameManager.instance.isPaused;
    }

    static void TickUnpaused(ref float _seconds)
    {
        if (!IsPaused())
            _seconds -= Time.unscaledDeltaTime;
    }

    static void EnforceActiveScaleIfNeeded()
    {
        float target = IsPaused() ? 0f : activeSlowScale;
        if (!Mathf.Approximately(Time.timeScale, target))
            Time.timeScale = target;
    }

    static void SetScaleRespectingPause(float _scale)
    {
        Time.timeScale = IsPaused() ? 0f : _scale;
    }

    // -*-*-*-*-*-*-*- Runner -*-*-*-*-*-*-*-
    static MonoBehaviour GetRunner()
    {
        if (runner != null) 
            return runner;

        GameObject go = GameObject.Find("PowerUpRunner");
        if (go == null)
        {
            go = new GameObject("PowerUpRunner");
            Object.DontDestroyOnLoad(go);
        }

        runner = go.GetComponent<Runner>();
        if (runner == null)
            runner = go.AddComponent<Runner>();

        return runner;
    }
    class Runner : MonoBehaviour { }

    // -*-*-*-*-*-*-*- Time Slow -*-*-*-*-*-*-*-

    static void ApplyTimeSlow(float _scale, int _durationSeconds)
    {
        _scale = Mathf.Clamp(_scale, 0.01f, 1f);
        if (_durationSeconds < 0)
            _durationSeconds = 0;

        activeSlowScale = Mathf.Min(activeSlowScale, _scale);
        slowRemainingSeconds = Mathf.Max(slowRemainingSeconds, _durationSeconds);

        SetScaleRespectingPause(activeSlowScale);

        if (slowRoutine == null)
            slowRoutine = GetRunner().StartCoroutine(TimeSlowRoutine());
    }

    static IEnumerator TimeSlowRoutine()
    {
        while (slowRemainingSeconds > 0f)
        {
            EnforceActiveScaleIfNeeded();
            TickUnpaused(ref slowRemainingSeconds);
            yield return null;
        }

        activeSlowScale = 1f;
        SetScaleRespectingPause(activeSlowScale);
        slowRoutine = null;
    }
}
