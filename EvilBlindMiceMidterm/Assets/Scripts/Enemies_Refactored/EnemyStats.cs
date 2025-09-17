using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // inspector variables
    public Rigidbody body;
    public int startingHealth;

    // private variables
    int health;


    // Unity Methods
    private void Start()
    {
        health = startingHealth;
    }



    // Modifiers
    public void ModifyHealth(int _amount)
    {
        health += _amount;
    }



    // Getters
    public int GetHealth() { return health; }
}
