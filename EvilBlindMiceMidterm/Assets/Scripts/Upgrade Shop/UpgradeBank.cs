using System.Collections.Generic;
using UnityEngine;

public static class UpgradeBank
{
    public static int maxHealthDelta = 0;
    public static int initialDashDelta = 0;
    public static int healAmountDelta = 0;
    public static readonly HashSet<WeaponStats> unlockedWeapons = new();

    public static void Clear()
    {
        maxHealthDelta = 0;
        initialDashDelta = 0;
        healAmountDelta = 0;
        unlockedWeapons.Clear();
    }
}
