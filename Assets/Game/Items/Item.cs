﻿namespace Items
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

    public enum ItemType
    {
        WEAPON,
        CONSUMABLE
    };


    public enum FoodType
    {
        SNACK = 5,
        MEAL = 20,
    }

    public enum ArmourType
    {
        PLATE = 5,
        VEST = 15,
        SUIT = 40
    }

    public enum AmmoType
    {
        SINGLE = 1,
        BUNDLE = 5,
        QUIVER = 15
    }
}