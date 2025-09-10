using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{
    public enum PickUpType
    {
        Heal, TimeSlow, Invincibility, SpeedBoost
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
        }

        Destroy(gameObject);
    }

    // -*-*-*-*-*-*-*- Helper Methods -*-*-*-*-*-*-*-

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
        if (GameManager.instance != null)
            return GameManager.instance;

        if (runner != null)
            return runner;

        GameObject go = new GameObject("PowerUpRunner");
        Object.DontDestroyOnLoad(go);
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
