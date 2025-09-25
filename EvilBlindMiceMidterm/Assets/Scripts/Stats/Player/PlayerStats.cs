using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    // This script is only for storage and manipulation of player stats and handling powerup states
    // Anything else is outside the scope of PlayerStats
    // Please do not create object references to gameObjects outside of the player prefab

    // VARIABLES

    [SerializeField] public int initialRunSpeed = 15;
    [SerializeField] int initialJumpForce = 15;
    [SerializeField] int initialMaxHealth;
    [SerializeField] int initialJumpMax = 1;
    [SerializeField] float initialDashForce = 50;
    [SerializeField] int initialDashCount = 0;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public float runSpeed;
    float dashForce;
    int dashCount; 
    float jumpForce;
    int jumpMax;
    [HideInInspector] public float distanceTraveled;

    public event System.Action<int> OnDashChanged;

    SpeedState currentSpeedState;
    InvincibilityState currentInvincibilityState;

    [HideInInspector] public int tripCounter = 0;
    [HideInInspector] public bool hasTripped = false;
    public static PlayerStats instance { get; private set; }



    // UNITY

    private void Awake()
    {
        instance = this;

        currentSpeedState = new NormalSpeedState();
        currentInvincibilityState = new InvincibilityOffState();

        currentSpeedState.Enter(this);
        currentInvincibilityState.Enter(this);
    }

    private void Start()
    {
        ResetRunSpeed();
        ResetJumpForce();
        ResetMaxHealth();
        ResetHealthToFull();
        ResetJumpMax();
        ResetDashForce();
        ResetDashCount();
    }

    private void Update()
    {
        float deltaSeconds = Time.unscaledDeltaTime;
        bool isPaused = IsPaused();

        currentSpeedState.Update(this, deltaSeconds, isPaused);
        currentInvincibilityState.Update(this, deltaSeconds, isPaused);
    }



    // REQUEST API

    public void RequestSpeedBoost(float _multiplier, float _durationSeconds)
    {
        if (_multiplier < 1f)
            _multiplier = 1f;

        if (_durationSeconds < 0)
            _durationSeconds = 0;

        currentSpeedState.RequestBoost(this, _multiplier, _durationSeconds);
    }

    public void RequestInvincibility(int _durationSeconds)
    {
        if (_durationSeconds < 0)
            _durationSeconds = 0;

        currentInvincibilityState.RequestInvincibility(this, _durationSeconds);
    }

    public void RequestTripState(float _multiplier, int _durationSeconds)
    {

        if (_durationSeconds < 0)
            _durationSeconds = 0;

        currentSpeedState.RequestTrip(this, _multiplier, _durationSeconds);
    }

    public bool IsInvincible() { return currentInvincibilityState is InvincibilityOnState; }



    // RESET

    public void ResetRunSpeed() { runSpeed = initialRunSpeed; }
    public void ResetJumpForce() { jumpForce = initialJumpForce; }
    public void ResetMaxHealth() { maxHealth = initialMaxHealth; }
    public void ResetHealthToFull() { currentHealth = maxHealth; }
    public void ResetJumpMax() { jumpMax = initialJumpMax; }
    public void ResetDashForce() { dashForce = initialDashForce; }
    public void ResetDashCount() 
    { 
        dashCount = initialDashCount;
        OnDashChanged?.Invoke(dashCount);
        GameManager.instance?.UpdatePlayerUI();
    }



    // MULTIPLIERS

    public void MultiplyRunSpeed(float _multiplier) { runSpeed = runSpeed * _multiplier; }
    public void MultiplyJumpForce(float _multiplier) { jumpForce = jumpForce * _multiplier; }



    // ADDITION

    public void AddMaxHealth(int _modifier)
    {
        maxHealth += _modifier;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void AddHealth(int _modifier)
    {
        int before = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + _modifier, 0, maxHealth);

        GameManager gameManager = GameManager.instance;
        if (gameManager != null)
        {
            gameManager.UpdatePlayerUI();

            if (currentHealth < before)
                gameManager.FlashDamage();
            else if (currentHealth >= before)
                gameManager.FlashHeal();
        }

        if (currentHealth <= 0 && gameManager != null)
            gameManager.YouLose();
    }

    public void AddJumpMax(int _modifier)
    {
        jumpMax += _modifier;
    }

    public void AddDistanceTraveled(float _distanceTravel)
    {
        distanceTraveled += _distanceTravel;
    }

    public void AddInitialDashCount(int _modifier)
    {
        initialDashCount += _modifier;
    }

    public void AddDashCount(int _modifier)
    {
        int before = dashCount;

        dashCount = Mathf.Max(0, dashCount + _modifier);

        if (dashCount != before)
        {
            OnDashChanged?.Invoke(dashCount);
            GameManager.instance?.UpdatePlayerUI();
        }
    }

    public bool TrySpendDash()
    {
        if (dashCount <= 0) 
            return false;

        dashCount--;
        OnDashChanged?.Invoke(dashCount);
        GameManager.instance?.UpdatePlayerUI();
        return true;
    }



    // GETTERS

    public float GetSpeed() { return runSpeed; }
    public float GetJumpForce() { return jumpForce; }
    public int GetHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    public int GetJumpMax() { return jumpMax; }
    public float GetDistanceTraveled() { return distanceTraveled; }
    public int GetDashCount() { return dashCount; }
    public float GetDashForce() {  return dashForce; }



    // HELPERS

    public void ResetAllPowerUpEffects()
    {
        TransitionToSpeedState(new NormalSpeedState());
        TransitionToInvincibilityState(new InvincibilityOffState());
    }

    static bool IsPaused()
    {
        return GameManager.instance != null && GameManager.instance.isPaused;
    }

    void TransitionToSpeedState(SpeedState _next)
    {
        currentSpeedState.Exit(this);
        currentSpeedState = _next;
        currentSpeedState.Enter(this);
    }

    void TransitionToInvincibilityState(InvincibilityState _next)
    {
        currentInvincibilityState.Exit(this);
        currentInvincibilityState = _next;
        currentInvincibilityState.Enter(this);
    }
    void TransitionToTripState(SpeedState _next)
    {
        currentSpeedState.Exit(this);
        currentSpeedState = _next;
        currentSpeedState.Enter(this);
    }



    // SPEED STATES


    /* Base class for all run-speed-related states.
     * The PlayerStats.Update() drives these with unscaled delta seconds,
     * so timers are unaffected by Time.timeScale (pause/slow) */
    abstract class SpeedState
    {
        public abstract void Enter(PlayerStats _stats);
        public abstract void Exit(PlayerStats _stats);
        public abstract void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused);
        public abstract void RequestBoost(PlayerStats _stats, float _multiplier, float _durationSeconds);
        public abstract void RequestTrip(PlayerStats _stats, float _multiplier, float _durationSeconds);
    }

    // Baseline movement: runSpeed equals initialRunSpeed. No timers.
    class NormalSpeedState : SpeedState
    {
        public override void Enter(PlayerStats _stats) { _stats.runSpeed = _stats.initialRunSpeed; } // Always snap back to baseline when entering normal
        public override void Exit(PlayerStats _stats) { }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused) { }
        public override void RequestBoost(PlayerStats _stats, float _multiplier, float _durationSeconds)
        {
            // Switch to a timed boosted state
            _stats.TransitionToSpeedState(new BoostedSpeedState(_multiplier, _durationSeconds, _stats.initialRunSpeed));
        }
        public override void RequestTrip(PlayerStats _stats, float _multiplier, float _durationSeconds)
        {
            _stats.TransitionToTripState(new TripState(_multiplier, _durationSeconds, _stats.initialRunSpeed));
        }
    }

    /* Temporary speed boost: runSpeed = baseRunSpeed * multiplier while active,
     * then returns to NormalSpeedState when time runs out */
    class BoostedSpeedState : SpeedState
    {
        float multiplier;
        float remainingSeconds;
        readonly float baseRunSpeed;

        /* Captured when the boost is created. Keeping a base avoids compounding
         * boosts on top of boosts and guarantees a clean Exit() revert */
        public BoostedSpeedState(float _multiplier, float _durationSeconds, float _baseRunSpeed)
        {
            multiplier = Mathf.Max(1f, _multiplier);
            remainingSeconds = Mathf.Max(0, _durationSeconds);
            baseRunSpeed = _baseRunSpeed;
        }

        public override void Enter(PlayerStats _stats) { _stats.runSpeed = baseRunSpeed * multiplier; }
        public override void Exit(PlayerStats _stats) { _stats.runSpeed = baseRunSpeed; }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused)
        {
            if (_isPaused) return;

            remainingSeconds -= _deltaSeconds;

            // Time's up -> back to normal
            if (remainingSeconds <= 0f)
                _stats.TransitionToSpeedState(new NormalSpeedState());
        }

        public override void RequestBoost(PlayerStats _stats, float _newMultiplier, float _newDurationSeconds)
        {
            /* Re-boost logic:
             * - If the new multiplier is stronger, apply it and extend/refresh duration.
             * - If equal, extend/refresh duration.
             * - If weaker, ignore. */
            if (_newMultiplier > multiplier)
            {
                multiplier = _newMultiplier;
                remainingSeconds = Mathf.Max(remainingSeconds, _newDurationSeconds);
                _stats.runSpeed = baseRunSpeed * multiplier;
            }
            else if (Mathf.Approximately(_newMultiplier, multiplier))
            {
                remainingSeconds = Mathf.Max(remainingSeconds, _newDurationSeconds);
            }
        }
        public override void RequestTrip(PlayerStats _stats, float _multiplier, float _durationSeconds) { }
    }

    class TripState : SpeedState
    {
        float multiplier;
        float remainingSeconds;
        readonly float baseRunSpeed;

        public TripState(float _multiplier, float _durationSeconds, float _baseRunSpeed)
        {
            multiplier = _multiplier;
            remainingSeconds = Mathf.Max(0, _durationSeconds);
            baseRunSpeed = _baseRunSpeed;
        }
        public override void Enter(PlayerStats _stats) { _stats.hasTripped = false; }
        public override void Exit(PlayerStats _stats) { _stats.hasTripped = false; }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused)
        {
            if (_isPaused) return;

            remainingSeconds -= _deltaSeconds;

            if (remainingSeconds <= 0f)
            {
                _stats.TransitionToSpeedState(new NormalSpeedState());
                _stats.tripCounter = 0;
            }


        }

        public override void RequestBoost(PlayerStats _stats, float _newMultiplier, float _newDurationSeconds)
        {

        }
        public override void RequestTrip(PlayerStats _stats, float _multiplier, float _durationSeconds)
        {
            if (!_stats.hasTripped)
            {
                //_stats.hasTripped = !_stats.hasTripped;
                multiplier = 0.5f;
                remainingSeconds = Mathf.Max(remainingSeconds, _durationSeconds);
                _stats.runSpeed = baseRunSpeed * multiplier;
            }
            else
            {
                _stats.currentHealth = 0;
                GameManager.instance.YouLose();
            }
        }
    }



    // INVINCIBILITY STATES

    // Base class for Invincibility
    abstract class InvincibilityState
    {
        public abstract void Enter(PlayerStats _stats);
        public abstract void Exit(PlayerStats _stats);
        public abstract void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused);
        /* Turn on / extend invincibility for the given duration
         * The active state decides whether to transition or extend */
        public abstract void RequestInvincibility(PlayerStats _stats, int _durationSeconds);
    }

    // Default: not invincible. Request switches to On-state with a timer.
    class InvincibilityOffState : InvincibilityState
    {
        public override void Enter(PlayerStats _stats) { }
        public override void Exit(PlayerStats _stats) { }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused) { }
        public override void RequestInvincibility(PlayerStats _stats, int _durationSeconds)
        {
            _stats.TransitionToInvincibilityState(new InvincibilityOnState(_durationSeconds));
        }
    }

    /* Invincible while the timer runs. When it expires, returns to Off-state.
     * PlayerDamageReceiver checks PlayerStats.IsInvincible() to gate damage. */
    class InvincibilityOnState : InvincibilityState
    {
        float remainingSeconds;

        public InvincibilityOnState(int _durationSeconds) { remainingSeconds = Mathf.Max(0, _durationSeconds); }
        public override void Enter(PlayerStats _stats) { }
        public override void Exit(PlayerStats _stats) { }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused)
        {
            if (_isPaused) return;

            remainingSeconds -= _deltaSeconds;
            if (remainingSeconds <= 0f)
                _stats.TransitionToInvincibilityState(new InvincibilityOffState());
        }
        public override void RequestInvincibility(PlayerStats _stats, int _durationSeconds)
        {
            if (_durationSeconds > remainingSeconds)
                remainingSeconds = _durationSeconds;
        }

    }
}
