﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class HitBoxPlayer : PunBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponent<Player>().ID != player.ID)
            {
                //col.gameObject.GetComponent<PlayerStats>().ReceiveDamage((int)(player._myPlayerStats.m_DamageMelee));

                if (col.gameObject.GetComponent<PlayerStats>().m_HP <= 0)
                {
                    player._myPlayerStats.KilledTarget(100);
                }
            }
        }
    }
}
