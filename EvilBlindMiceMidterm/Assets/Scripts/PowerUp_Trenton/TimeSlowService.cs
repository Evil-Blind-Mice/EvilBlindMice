using System.Collections;
using UnityEngine;

public static class TimeSlowService
{
    static float activeSlowScale = 1;
    static float slowRemainingSeconds = 0;
    static Coroutine slowRoutine;
    static MonoBehaviour runner;

    static bool IsPaused => GameManager.instance != null && GameManager.instance.isPaused;

    public static void Apply(float _scale, int _durationSeconds)
    {
        _scale = Mathf.Clamp(_scale, 0.01f, 1);
        if (_durationSeconds < 0)
            _durationSeconds = 0;

        // lowest scale wins, longest duration wins
        activeSlowScale = Mathf.Min(activeSlowScale, _scale);
        slowRemainingSeconds = Mathf.Max(slowRemainingSeconds, _durationSeconds);

        SetScaleRespectingPause(activeSlowScale);

        if (slowRoutine == null)
            slowRoutine = EnsureRunner().StartCoroutine(TimeSlowRoutine());
    }

    public static void Reset()
    {
        // stop ticking
        if (slowRoutine != null)
        {
            EnsureRunner().StopCoroutine(slowRoutine);
            slowRoutine = null;
        }

        activeSlowScale = 1;
        slowRemainingSeconds = 0;
        SetScaleRespectingPause(1);
    }

    // -_-_-_-_- Internals -_-_-_-_-

    static void TickUnpaused(ref float _seconds)
    {
        if (!IsPaused)
            _seconds -= Time.unscaledDeltaTime;
    }

    static void EnforceActiveScaleIfNeeded()
    {
        float target = IsPaused ? 0 : activeSlowScale;
        if (!Mathf.Approximately(Time.timeScale, target))
            Time.timeScale = target;
    }

    static void SetScaleRespectingPause(float _scale)
    {
        Time.timeScale = IsPaused ? 0 : _scale;
    }

    static MonoBehaviour EnsureRunner()
    {
        if (runner != null) return runner;

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

    static IEnumerator TimeSlowRoutine()
    {
        while (slowRemainingSeconds > 0)
        {
            EnforceActiveScaleIfNeeded();
            TickUnpaused(ref slowRemainingSeconds);
            yield return null;
        }

        activeSlowScale = 1;
        SetScaleRespectingPause(activeSlowScale);
        slowRoutine = null;
    }
}
