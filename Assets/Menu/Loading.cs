using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //Esta es la forma correcta de mostrar variables privadas en el inspector
    //No se deben hacer public variables que no queremos sean accesibles desde otras clases
    [SerializeField]
    private string sceneToLoad;
    public Text NickName;

    public void LoadScene()
    {
        Debug.Log("Hey");
        if (NickName.text == null || NickName.text == "")
        {
            PhotonNetwork.player.NickName = "JugadorRandom" + Random.Range(0, 99999);
        }
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
    }  
}
