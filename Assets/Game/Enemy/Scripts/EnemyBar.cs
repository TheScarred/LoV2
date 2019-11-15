using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class EnemyBar : PunBehaviour
{
    float damage_percentage;
    public Image bar_HP;

    public void ModifyHpBar(float damage, int base_HP)
    {
        damage_percentage = (damage / base_HP);
        bar_HP.fillAmount -= damage_percentage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(bar_HP.fillAmount);
        }
        else
        {
            bar_HP.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
