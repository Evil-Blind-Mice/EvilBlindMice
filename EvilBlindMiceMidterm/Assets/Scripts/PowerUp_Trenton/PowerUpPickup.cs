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
    [SerializeField] float slowDurationSeconds;

    [Header("Invincibility")]
    [SerializeField] float invincibilityDurationSeconds;

    [Header("Speed Boost")]
    [SerializeField, Min(1)] int speedMultiplier;
    [SerializeField] float speedDurationSeconds;

    static float activeSlowScale = 1f;
    static float slowRemain = 0f;
    static Coroutine slowRoutine;

    static float invincibilityRemain = 0f;
    static Coroutine invincibilityRoutine;

    static int activeSpeedMultiplier = 1;
    static float speedRemain = 0f;
    static Coroutine speedRoutine;

    static MonoBehaviour runner;

    void OnTriggerEnter(Collider _other)
    {
        if (_other.isTrigger) return;
        if (!_other.CompareTag("Player")) return;

        PlayerController player = _other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        switch (type)
        {
            case PickUpType.Heal:
                player.Heal(healAmount);
                break;

            case PickUpType.TimeSlow:
                ApplySlowMotion(slowScale, slowDurationSeconds);
                break;

            case PickUpType.Invincibility:
                ApplyInvincibility(invincibilityDurationSeconds);
                break;

            case PickUpType.SpeedBoost:
                ApplySpeedBoost(speedMultiplier, speedDurationSeconds);
                break;
        }

        Destroy(gameObject);
    }

    // -*-*-*-*-*-*-*- Helper Methods -*-*-*-*-*-*-*-

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

    static void ApplySlowMotion(float _scale, float _duration)
    {
        _scale = Mathf.Clamp(_scale, 0.01f, 1f);
        _duration = Mathf.Max(0f, _duration);

        activeSlowScale = Mathf.Min(activeSlowScale, _scale);
        slowRemain = Mathf.Max(slowRemain, _duration);

        SetScaleRespectingPause(activeSlowScale);

        if (slowRoutine == null)
            slowRoutine = GetRunner().StartCoroutine(SlowMotionRoutine());
    }

    static IEnumerator SlowMotionRoutine()
    {
        while (slowRemain > 0f)
        {
            EnforceActiveScaleIfNeeded();
            TickUnpaused(ref slowRemain);
            yield return null;
        }

        activeSlowScale = 1f;
        SetScaleRespectingPause(activeSlowScale);
        slowRoutine = null;
    }

    // -*-*-*-*-*-*-*- Invincibility -*-*-*-*-*-*-*-

    static void ApplyInvincibility(float _duration)
    {
        _duration = Mathf.Max(0f, _duration);
        invincibilityRemain = Mathf.Max(invincibilityRemain, _duration);
        SetInvincible(true);

        if (invincibilityRoutine == null)
            invincibilityRoutine = GetRunner().StartCoroutine(InvincibilityRoutine());
    }

    static IEnumerator InvincibilityRoutine()
    {
        while (invincibilityRemain > 0f)
        {
            TickUnpaused(ref invincibilityRemain);
            yield return null;
        }

        SetInvincible(false);
        invincibilityRoutine = null;
    }

    static void SetInvincible(bool _on)
    {
        if (GameManager.instance == null) return;
        if (GameManager.instance.playerScript == null) return;
        GameManager.instance.playerScript.isInvincible = _on;
    }

    // -*-*-*-*-*-*-*- Speed Boost -*-*-*-*-*-*-*-

    static void ApplySpeedBoost(int _multiplier, float _duration)
    {
        _multiplier = Mathf.Max(1, _multiplier);
        _duration = Mathf.Max(0f, _duration);

        if (_multiplier > activeSpeedMultiplier)
        {
            activeSpeedMultiplier = _multiplier;
            SetSpeedBoostOnPlayer(activeSpeedMultiplier);
            speedRemain = Mathf.Max(speedRemain, _duration);
        }
        else if (Mathf.Approximately(_multiplier, activeSpeedMultiplier))
        {
            speedRemain = Mathf.Max(speedRemain, _duration);
        }
        else
        {
            return;
        }

        if (speedRoutine == null)
            speedRoutine = GetRunner().StartCoroutine(SpeedBoostRoutine());
    }

    static IEnumerator SpeedBoostRoutine()
    {
        while (speedRemain > 0f)
        {
            TickUnpaused(ref speedRemain);
            yield return null;
        }

        activeSpeedMultiplier = 1;
        SetSpeedBoostOnPlayer(activeSpeedMultiplier);
        speedRoutine = null;
    }

    static void SetSpeedBoostOnPlayer(int _multiplier)
    {
        if (GameManager.instance == null) return;
        if (GameManager.instance.playerScript == null) return;

        GameManager.instance.playerScript.SetSpeedBoostMultiplier(_multiplier);
    }
}
