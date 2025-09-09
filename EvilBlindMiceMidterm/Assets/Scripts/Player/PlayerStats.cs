using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] int initialRunSpeed;
    [SerializeField] int initialJumpForce;
    [SerializeField] int initialMaxHealth;
    int maxHealth;
    int currentHealth;
    float runSpeed;
    float jumpForce;
    public static PlayerStats instance;

    // UNITY

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetRunSpeed();
        ResetJumpForce();
        ResetMaxHealth();
        ResetHealthToFull();
    }



    // RESET

    public void ResetRunSpeed()
    {
        runSpeed = initialRunSpeed;
    }

    public void ResetJumpForce()
    {
        jumpForce = initialJumpForce;
    }
    
    public void ResetMaxHealth()
    {
        maxHealth = initialMaxHealth;
    }

    public void ResetHealthToFull()
    {
        currentHealth = maxHealth;
    }



    // MULTIPLYERS

    public void MultiplyRunSpeed(float _multiplier)
    {
        runSpeed = runSpeed * _multiplier;
    }

    public void MultiplyJumpForce(float _multiplier)
    {
        jumpForce = jumpForce * _multiplier;
    }



    // ADDITION

    public void AddMaxHealth(int _modifier)
    {
        maxHealth += _modifier;
    }

    public void AddHealth(int _modifier)
    {
        currentHealth += _modifier;
    }
}
