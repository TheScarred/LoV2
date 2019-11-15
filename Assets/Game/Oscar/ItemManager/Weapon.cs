using UnityEngine;
using Items;

[CreateAssetMenu(fileName = "New Player Weapon", menuName = "Weapons")]
public class Weapon : ScriptableObject
{
    public Sprite sprite;
    public WeaponType type;
    public WeaponRarity rarity;
    public WeaponStats.Stats stats;

    // Funcion para resetear el arma a la basica cuando otra se rompa.
    public void OnWeaponBreak(WeaponStats.Stats stats, WeaponType type)
    {
        if (type == WeaponType.MELEE)
            WeaponStats.SetBasicMelee(stats);
        else
            WeaponStats.SetBasicRanged(stats);
    }
}
