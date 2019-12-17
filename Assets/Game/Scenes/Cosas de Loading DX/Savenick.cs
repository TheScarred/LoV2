using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class Savenick : PunBehaviour
{
    public InputField input, NickName;
    public Image Viking1;
    public Image Viking2;
    public static string nombrePJ;
    string Name;
    string UIImagen;

    public GameObject buttonstart;
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        buttonstart.SetActive(true);
    }

    public void SaveNickName()
    { 
        Name = NickName.text;


        PhotonNetwork.player.NickName = Name;
    
    }

    public void GuardarPJ()
    {
        if (Viking1.enabled)
            UIImagen = "Viking1";
        else
            UIImagen = "Viking2";
    }
    public void AsignarPJ()
    {
        if (UIImagen == "Viking2")
        {            
            nombrePJ = "PlayerNetN";
        }
        if (UIImagen == "Viking1")
        {
            nombrePJ = "PlayerNetIvan";
        }
    }
}
