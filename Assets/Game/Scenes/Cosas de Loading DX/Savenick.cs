using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class Savenick : PunBehaviour
{
    public static string nombrePJ;
    string UIImagen;
    public string NickName;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        
    }
    public void SaveNickName()
    {
        NickName = GameObject.Find("NickName").GetComponent<Text>().text.ToString();

        if(NickName ==null)
        {

        }

        PhotonNetwork.player.NickName = NickName;

    }

    public void GuardarPJ()
    {
        
        UIImagen = GameObject.Find("ImageCambiante").GetComponent<Image>().sprite.name;
       
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
