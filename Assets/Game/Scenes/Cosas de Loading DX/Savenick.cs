using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class Savenick : PunBehaviour
{
    public static string nombrePJ;
    string UIImagen;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        
    }
    public void SaveNickName()
    {
        string NickName = GameObject.Find("NickName").GetComponent<Text>().text.ToString();
        PhotonNetwork.player.NickName = NickName;

    }

    public void GuardarPJ()
    {
        
        UIImagen = GameObject.Find("ImageCambiante").GetComponent<Image>().sprite.name;
       
    }
    public void AsignarPJ()
    {
        if (UIImagen == "zeropj")
        {
            
            nombrePJ = "PlayerNetN";
        }
        if (UIImagen == "Knight_idle_01")
        {
            
            nombrePJ = "PlayerNetIvan";
        }
    }
}
