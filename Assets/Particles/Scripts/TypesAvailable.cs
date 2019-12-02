using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypesAvailable : MonoBehaviour
{
    public enum particleType
    {
        HEAL,
        PLAYER_DEATH,
        PLAYER_SPAWN,
        HIT,
        ENEMY_DEATH,
        GRAB_WEAPON,
        MOD_KNOCKBACK,
        MOD_DEFENSE,
        MOD_ATTACK,
        MOD_SPEED,
        MOD_BLEED,
    }

    public bool timer;  //timer to deactivation
}
