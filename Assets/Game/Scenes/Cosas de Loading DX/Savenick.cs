using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class Savenick : PunBehaviour
{
    public InputField input, NickName;
    public Image imagenCambiante;
    public static string nombrePJ;
    string Name;
    string UIImagen;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveNickName()
    {
        Name = NickName.text;
        PhotonNetwork.player.NickName = Name;
       
    }

    public void GuardarPJ()
    {
        UIImagen = imagenCambiante.sprite.name;
    }
    public void AsignarPJ()
    {
        if (UIImagen == "Viking2P")
        {            
            nombrePJ = "PlayerNetN";
        }
        if (UIImagen == "Viking1P")
        {
            nombrePJ = "PlayerNetIvan";
        }
    }
}
