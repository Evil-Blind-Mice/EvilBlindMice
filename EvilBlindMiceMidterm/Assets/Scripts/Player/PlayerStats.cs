using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // VARIABLES

    [SerializeField] int initialRunSpeed = 15;
    [SerializeField] int initialJumpForce = 15;
    [SerializeField] int initialMaxHealth;
    [SerializeField] int initialJumpMax = 1;
    int maxHealth;
    int currentHealth;
    float runSpeed;
    float jumpForce;
    int jumpMax;
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
        ResetJumpMax();
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

    public void ResetJumpMax()
    {
        jumpMax = initialJumpMax;
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

    public void AddJumpMax(int _modifier)
    {
        jumpMax += _modifier;
    }



    // GETTERS
    
    public float GetSpeed()
    {
        return runSpeed;
    }

    public float GetJumpForce()
    {
        return jumpForce;
    }
    
    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetJumpMax()
    {
        return jumpMax;
    }
}
