using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour, IDamage
{
    [SerializeField] EnemyStats stats;

    public void TakeDamage(int _amount)
    {
        stats.ModifyHealth(-_amount);
        if (stats.GetHealth() <= 0)
            Destroy(stats.gameObject);
    }
}
