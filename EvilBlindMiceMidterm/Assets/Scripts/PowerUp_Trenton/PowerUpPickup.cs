using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] int healAmount;

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        var player = _other.GetComponent<PlayerController>();
        if (!player) return;

        player.Heal(healAmount);
        Destroy(gameObject);
    }
}
