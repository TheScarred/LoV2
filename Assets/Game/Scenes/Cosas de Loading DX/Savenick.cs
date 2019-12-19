using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using UnityEngine.SceneManagement;
public class Savenick : PunBehaviour
{
    public InputField input, NickName;
    public Image Viking1;
    public Image Viking2;
    public static string nombrePJ;
    string Name;
    string UIImagen;

    public GameObject buttonstart;
    private static Savenick instance;

    public static Savenick GetInstance()
    {
        return instance;
    }
    void Start()
    {
        if(instance==null)
        {
            instance = this;
        }
        else if(instance!=null)
        {
            
            

        }
        DontDestroyOnLoad(this.gameObject);
        buttonstart.SetActive(true);
    }

    public void SaveNickName()
    { 
        Name = NickName.text;


        PhotonNetwork.player.NickName = Name;
    
    }
    public void Update()
    {
        
    }
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name == "Game Over")
        {
            Destroy(gameObject);
        
        }
        Debug.Log(mode);
    }

  
    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
