using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{
    public enum PickUpType
    {
        Heal, TimeSlow
    }

    [Header("PowerUp Type")]
    [SerializeField] PickUpType type = PickUpType.Heal;

    [Header("Heal")]
    [SerializeField] int healAmount;

    [Header("Time Slow")]
    [SerializeField, Range(0.05f, 1f)] float slowScale;
    [SerializeField] float slowDurationSeconds;

    static float activeSlowScale = 1f;
    static float slowRemain = 0f;
    static Coroutine slowRoutine;
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
        }

        Destroy(gameObject);
    }

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

    static void ApplySlowMotion(float _scale, float _duration)
    {
        bool paused = (GameManager.instance != null && GameManager.instance.isPaused);

        _scale = Mathf.Clamp(_scale, 0.01f, 1f);

        activeSlowScale = Mathf.Min(activeSlowScale, _scale);
        slowRemain = Mathf.Max(slowRemain, _duration);

        Time.timeScale = paused ? 0f : activeSlowScale;

        if (slowRoutine == null)
            slowRoutine = GetRunner().StartCoroutine(SlowMotionRoutine());
    }

    static IEnumerator SlowMotionRoutine()
    {
        while (slowRemain > 0f)
        {
            bool paused = (GameManager.instance != null && GameManager.instance.isPaused);
            float target = paused ? 0f : activeSlowScale;

            if (!Mathf.Approximately(Time.timeScale, target))
                Time.timeScale = target;

            if (!paused)
                slowRemain -= Time.unscaledDeltaTime;

            yield return null;
        }

        activeSlowScale = 1f;

        bool stillPaused = (GameManager.instance != null && GameManager.instance.isPaused);
        Time.timeScale = stillPaused ? 0f : 1f;

        slowRoutine = null;
    }
}
