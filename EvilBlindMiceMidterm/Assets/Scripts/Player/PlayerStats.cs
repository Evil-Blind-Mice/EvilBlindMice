using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // VARIABLES

    [SerializeField] public int initialRunSpeed = 15;
    [SerializeField] int initialJumpForce = 15;
    [SerializeField] int initialMaxHealth;
    [SerializeField] int initialJumpMax = 1;
    public int maxHealth;
    public int currentHealth;
    public float runSpeed;
    float jumpForce;
    int jumpMax;

    SpeedState currentSpeedState;
    InvincibilityState currentInvincibilityState;

    [HideInInspector] public int hasTripped = 0;
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
    }

    private void Update()
    {
        float deltaSeconds = Time.unscaledDeltaTime;
        bool isPaused = IsPaused();

        currentSpeedState.Update(this, deltaSeconds, isPaused);
        currentInvincibilityState.Update(this, deltaSeconds, isPaused);
    }



    // REQUEST API

    public void RequestSpeedBoost(float _multiplier, int _durationSeconds)
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

    public bool IsInvincible() { return currentInvincibilityState is InvincibilityOnState; }




    // RESET

    public void ResetRunSpeed() { runSpeed = initialRunSpeed; }
    public void ResetJumpForce() { jumpForce = initialJumpForce; }
    public void ResetMaxHealth() { maxHealth = initialMaxHealth; }
    public void ResetHealthToFull() { currentHealth = maxHealth; }
    public void ResetJumpMax() { jumpMax = initialJumpMax; }



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
        currentHealth = Mathf.Clamp(currentHealth + _modifier, 0, maxHealth);
    }

    public void AddJumpMax(int _modifier)
    {
        jumpMax += _modifier;
    }



    // GETTERS
    
    public float GetSpeed() { return runSpeed; }
    public float GetJumpForce() { return jumpForce; }
    public int GetHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    public int GetJumpMax() { return jumpMax; }



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



    // SPEED STATES

    abstract class SpeedState
    {
        public abstract void Enter(PlayerStats _stats);
        public abstract void Exit(PlayerStats _stats);
        public abstract void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused);
        public abstract void RequestBoost(PlayerStats _stats, float _multiplier, int _durationSeconds);
    }

    class NormalSpeedState : SpeedState
    {
        public override void Enter(PlayerStats _stats) { _stats.runSpeed = _stats.initialRunSpeed; }
        public override void Exit(PlayerStats _stats) { }
        public override void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused) { }
        public override void RequestBoost(PlayerStats _stats, float _multiplier, int _durationSeconds)
        {
            _stats.TransitionToSpeedState(new BoostedSpeedState(_multiplier, _durationSeconds, _stats.initialRunSpeed));
        }
    }

    class BoostedSpeedState : SpeedState
    {
        float multiplier;
        float remainingSeconds;
        readonly float baseRunSpeed;

        public BoostedSpeedState(float _multiplier, int _durationSeconds, float _baseRunSpeed)
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

            if (remainingSeconds <= 0f)
                _stats.TransitionToSpeedState(new NormalSpeedState());
        }

        public override void RequestBoost(PlayerStats _stats, float _newMultiplier, int _newDurationSeconds)
        {
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
    }



    // INVINCIBILITY STATES

    abstract class InvincibilityState
    {
        public abstract void Enter(PlayerStats _stats);
        public abstract void Exit(PlayerStats _stats);
        public abstract void Update(PlayerStats _stats, float _deltaSeconds, bool _isPaused);
        public abstract void RequestInvincibility(PlayerStats _stats, int _durationSeconds);
    }

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
