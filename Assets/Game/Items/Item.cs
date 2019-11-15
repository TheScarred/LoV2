using UnityEngine;
using System.Collections;

namespace Items
{
    public enum WeaponType
    {
        MELEE,
        RANGED
    };

    public enum WeaponRarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    };

    public enum Modifier
    {
        NONE,
        /*
        Melee -------------------------------------------------------------------
        */
        FRENZY,         // rate of fire +
        ARMOURSLAYER,   // % armour penetration +           
        FOCUS,          // critical % +                             {1-5}
        DURABILITY,     // wear +
        SWIFTNESS,      // movement spd +
        // ---------------------------------------- Legendary
        BLEEDING,       // x seconds of Bleeding Dmg
        BLOODTHIRST,   // placeholder                             {6-8}
        KNOCKBACK,   // placeholder
        /*
        Ranged -------------------------------------------------------------------
        */
        LOWDRAG,        // projectile spd +
        SPEEDLOAD,      // rate of fire +                           {9-12}
        POISON,         // x seconds of Poison Dmg
        BULLETPOINT,     // projectile range +
        // ----------------------------------------- Legendary
        EXPLOSIVE,
        HOMING                                                   // {13-14}
    };

    public enum ConsumeableType
    {
        AMMO,
        HEALTH,
        ARMOUR
    };

    public enum GoldType
    {
        COIN,
        BAG,
        BAR,
        CHEST
    };

    public enum FoodType
    {
        SMALL,
        MEDIUM,
        LARGE
    }

}