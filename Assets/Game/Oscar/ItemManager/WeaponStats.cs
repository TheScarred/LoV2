using UnityEngine;
using Items;

//[System.Serializable]
public class WeaponStats
{
    #region SINGLETON
    private static WeaponStats _instance;
    public static WeaponStats GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [System.Serializable]
    public struct Stats
    {
        public int id;
        public int wear;
        public float damage;
        public float rOF;
        public float critChance;
        public float armourPen;
        public Modifier mod1;
        public Modifier mod2;
        public string weaponName;
    }

    public static Stats SetBasicMelee(Stats stats)
    {
        stats.id = -1;
        stats.wear = -1;
        stats.damage = 5;
        stats.rOF = 1;
        stats.critChance = 0.05f;
        stats.armourPen = 0.25f;
        stats.weaponName = "Sword";
        return stats;
    }

    public static Stats SetBasicRanged(Stats stats)
    {
        stats.id = 2;
        stats.wear = 10;
        stats.damage = 2.5f;
        stats.rOF = 1;
        stats.critChance = 0;
        stats.armourPen = 0.1f;
        stats.weaponName = "Bow";
        return stats;
    }

    public static Stats SetStats(Stats stats, int seed, WeaponType type, WeaponRarity rarity, int id)
    {
        Random.InitState(seed);
        stats.id = id;

        if (type == WeaponType.MELEE)
        {
            if (rarity == WeaponRarity.UNCOMMON)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.075f, 0.3f, 5.5f, 6.5f);
            }
            else if (rarity == WeaponRarity.RARE)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.1f, 0.35f, 6.0f, 7.0f);
            }
            else if (rarity == WeaponRarity.EPIC)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.125f, 0.4f, 6.5f, 7.5f);
            }
            else if (rarity == WeaponRarity.LEGENDARY)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.15f, 0.45f, 7.0f, 8.0f);
            }
            else
            {
                SetAll(ref stats, type, rarity, -1, 1, 0.05f, 0.25f, 5f, 5f);
            }
        }
        else
        {
            if (rarity == WeaponRarity.UNCOMMON)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.075f, 0.1f, 3.0f, 3.5f);
            }
            else if (rarity == WeaponRarity.RARE)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.1f, 0.15f, 3.5f, 4.0f);
            }
            else if (rarity == WeaponRarity.EPIC)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.125f, 0.2f, 4.0f, 4.5f);
            }
            else if (rarity == WeaponRarity.LEGENDARY)
            {
                SetAll(ref stats, type, rarity, 30, 1, 0.15f, 0.25f, 4.5f, 5.0f);
            }
            else
            {
                SetAll(ref stats, type, rarity, 10, 1, 0.05f, 0.05f, 2.5f, 2.5f);
            }
        }

        return stats;
    }

    public static void SetWear(ref Stats stats, int amount)
    {
        stats.wear = amount;

    }

    public static void SetRateOfFire(ref Stats stats, float amount)
    {
        stats.rOF = amount;
    }

    public static void SetCritChance(ref Stats stats, float amount)
    {
        stats.critChance = amount;
    }

    public static void SetArmourPenetration(ref Stats stats, float amount)
    {
        stats.armourPen = amount;
    }

    public static void SetRandomDamage(ref Stats stats, float min, float max)
    {
        float damage = Random.Range(min, max);
        float step = Mathf.FloorToInt(damage / 0.1f);
        float newDamage = step * 0.1f;
        stats.damage = newDamage;
    }

    public static void SetAll(ref Stats stats, WeaponType type, WeaponRarity rarity, int wear, float rof, float crit, float armour, float damageMin, float damageMax)
    {
        SetWear(ref stats, wear);
        SetRateOfFire(ref stats, rof);
        SetCritChance(ref stats, crit);
        SetArmourPenetration(ref stats, armour);
        SetRandomDamage(ref stats, damageMin, damageMax);
        SetMods(ref stats, type, rarity);
    }

    public static void SetMods(ref Stats stats, WeaponType type, WeaponRarity weaponRarity)
    {
        if (weaponRarity == WeaponRarity.UNCOMMON)
        {
            stats.mod1 = Modifier.NONE;
            stats.mod2 = Modifier.NONE;
        }
        else if (weaponRarity == WeaponRarity.RARE)
        {
            if (type == WeaponType.MELEE)
            {
                stats.mod1 = (Modifier)Random.Range(1, 6);
                SetMeleeModifier(ref stats, stats.mod1);
            }
            else
            {
                stats.mod1 = (Modifier)Random.Range(9, 13);
                SetRangedModifier(ref stats, stats.mod1);
            }
            stats.mod2 = Modifier.NONE;
        }
        else if (weaponRarity == WeaponRarity.EPIC)
        {
            if (type == WeaponType.MELEE)
            {
                stats.mod1 = (Modifier)Random.Range(1, 6);
                stats.mod2 = (Modifier)Random.Range(1, 6);
                SetMeleeModifier(ref stats, stats.mod1);
                SetMeleeModifier(ref stats, stats.mod2);
            }
            else
            {
                stats.mod1 = (Modifier)Random.Range(9, 13);
                stats.mod2 = (Modifier)Random.Range(9, 13);
                SetRangedModifier(ref stats, stats.mod1);
                SetRangedModifier(ref stats, stats.mod2);
            }
        }
        else if (weaponRarity == WeaponRarity.LEGENDARY)
        {
            if (type == WeaponType.MELEE)
            {
                stats.mod1 = (Modifier)Random.Range(6, 9);
                stats.mod2 = (Modifier)Random.Range(1, 6);
                SetMeleeModifier(ref stats, stats.mod1);
                SetMeleeModifier(ref stats, stats.mod2);
            }
            else
            {
                stats.mod1 = (Modifier)Random.Range(13, 15);
                stats.mod2 = (Modifier)Random.Range(9, 13);
                SetRangedModifier(ref stats, stats.mod1);
                SetRangedModifier(ref stats, stats.mod2);
            }
        }
    }

    public static void SetMeleeModifier(ref Stats stats, Modifier mod)
    {
        switch (mod)
        {
            case (Modifier.FRENZY):
                {
                    SetFrenzyMod(ref stats);
                    break;
                }
            case (Modifier.ARMOURSLAYER):
                {
                    SetArmourSlayerMod(ref stats);
                    break;
                }
            case (Modifier.FOCUS):
                {
                    SetFocusMod(ref stats);
                    break;
                }
            case (Modifier.DURABILITY):
                {
                    SetDurabilityMod(ref stats);
                    break;
                }
            case (Modifier.SWIFTNESS):
                {
                    SetSwiftnessMod(ref stats);
                    break;
                }
            case (Modifier.BLEEDING):
                {
                    SetBleedingMod(ref stats);
                    break;
                }
            case (Modifier.BLOODTHIRST):
                {
                    SetBloodThirstMod(ref stats);
                    break;
                }
            case (Modifier.KNOCKBACK):
                {
                    SetKnockBackMod(ref stats);
                    break;
                }
        }
    }

    public static void SetRangedModifier(ref Stats stats, Modifier mod)
    {
        switch (mod)
        {
            case (Modifier.LOWDRAG):
                {
                    SetLowDragMod(ref stats);
                    break;
                }
            case (Modifier.SPEEDLOAD):
                {
                    SetSpeedLoadMod(ref stats);
                    break;
                }
            case (Modifier.POISON):
                {
                    SetPoisonMod(ref stats);
                    break;
                }
            case (Modifier.BULLETPOINT):
                {
                    SetBulletPoint(ref stats);
                    break;
                }
            case (Modifier.EXPLOSIVE):
                {
                    SetExplosiveMod(ref stats);
                    break;
                }
            case (Modifier.HOMING):
                {
                    SetHomingMod(ref stats);
                    break;
                }
        }
    }

    #region MODS
    public static void SetArmourSlayerMod(ref Stats stats)
    {
        stats.armourPen += 0.3f;
    }
    public static void SetFrenzyMod(ref Stats stats)
    {
        stats.rOF -= 0.15f;
    }

    public static void SetFocusMod(ref Stats stats)
    {
        stats.critChance += 0.15f;
    }

    public static void SetDurabilityMod(ref Stats stats)
    {
        stats.wear += 20;
    }
    public static void SetSwiftnessMod(ref Stats stats)
    {

    }
    public static void SetBleedingMod(ref Stats stats)
    {
        // player bleeding state
    }
    public static void SetBloodThirstMod(ref Stats stats)
    {
        // player hp + x when killing
    }
    public static void SetKnockBackMod(ref Stats stats)
    {
        // hit moves enemy back
    }
    public static void SetLowDragMod(ref Stats stats)
    {

    }
    public static void SetSpeedLoadMod(ref Stats stats)
    {
        stats.rOF -= 0.15f;
    }
    public static void SetPoisonMod(ref Stats stats)
    {
        //player poison state 5 sec
    }
    public static void SetBulletPoint(ref Stats stats)
    {
        //projectile range ++
    }
    public static void SetExplosiveMod(ref Stats stats)
    {
        //arrow explodes
    }
    public static void SetHomingMod(ref Stats stats)
    {
        //arrow follows nearest player (range)
    }
    #endregion
}