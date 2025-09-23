using UnityEngine;
using UnityEngine.Events;

public class UpgradeShop : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] PlayerShooting shooter;
    [SerializeField] PlayerStats stats;

    [Header("Shop Inventory")]
    [SerializeField] ShopItem[] items;

    [System.Serializable]
    public enum ItemType { IncreaseMaxHealth, IncreaseHealAmount, IncreaseInitialDashCharges, Weapon }

    [System.Serializable]
    public struct ShopItem
    {
        public string displayName;
        public ItemType type;

        [Tooltip("IncreaseMaxHealth / IncreaseInitialDashCharges / IncreaseHealAmount")]
        public int amount;

        [Tooltip("For IncreaseHealAmount")]
        public PowerUpStats healPowerupToBuff;

        [Tooltip("For Weapon")]
        public WeaponStats weapon;

        [Header("Purchase Rules & Feedback")]
        public bool oneTimeOnly;
        [HideInInspector] public bool purchased;
        public UnityEvent onPurchased;
    }

    void Awake()
    {
        if (!shooter) shooter = FindAnyObjectByType<PlayerShooting>();
        if (!stats) stats = FindAnyObjectByType<PlayerStats>();

        RefreshOwnedWeapons();
    }

    public void Buy(int _index)
    {
        if (_index < 0 || _index >= items.Length) return;

        ShopItem item = items[_index];

        if (item.oneTimeOnly && item.purchased) return;

        bool success = ApplyItem(item);
        if (!success) return;

        if (item.oneTimeOnly)
        {
            item.purchased = true;
            items[_index] = item;
        }

        item.onPurchased?.Invoke();
        GameManager.instance?.UpdatePlayerUI();
    }

    bool ApplyItem(ShopItem _item)
    {
        switch (_item.type)
        {
            case ItemType.IncreaseMaxHealth:
                if (!stats) return false;
                stats.AddMaxHealth(_item.amount);
                return true;

            case ItemType.IncreaseHealAmount:
                if (!_item.healPowerupToBuff || _item.healPowerupToBuff.type != PowerUpType.Heal) return false;
                _item.healPowerupToBuff.healAmount += _item.amount;
                return true;

            case ItemType.IncreaseInitialDashCharges:
                if (!stats) return false;
                stats.AddInitialDashCount(_item.amount);
                return true;

            case ItemType.Weapon:
                if (!shooter || !_item.weapon) return false;

                // Already own
                if (shooter.weaponList.Contains(_item.weapon)) return true;

                // Don't own
                _item.weapon.weaponCurrentAmmo = _item.weapon.weaponMaxAmmo;
                shooter.GetWeaponStats(_item.weapon);
                return true;

        }

        return false;
    }

    public void RefreshOwnedWeapons()
    {
        if (!shooter || items == null) return;

        for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
        {
            if (items[itemIndex].oneTimeOnly && items[itemIndex].type == ItemType.Weapon && items[itemIndex].weapon)
            {
                if (shooter.weaponList.Contains(items[itemIndex].weapon))
                {
                    ShopItem item = items[itemIndex];
                    item.purchased = true;
                    items[itemIndex] = item;
                }
            }
        }
    }
}
