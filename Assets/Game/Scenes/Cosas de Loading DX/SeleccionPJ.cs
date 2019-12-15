using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SeleccionPJ : MonoBehaviour
{
    public Image Viking;
   public void Start()
    {
        Viking = GameObject.Find("ImageCambiante").GetComponent<Image>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Viking.sprite = Resources.Load<Sprite>("Sprites/zeropj");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Viking.sprite = Resources.Load<Sprite>("Sprites/Knight_idle_01");
        }
    }
    public void FlechitaDerecha()
     {
            Viking.sprite = Resources.Load<Sprite>("Sprites/Viking2P");
    }
     public void FlechitaIzquierda()
        {
           Viking.sprite = Resources.Load<Sprite>("Sprites/Viking1P");
        }
    /*public void Arrow()
    {
        print("Call");
        if (Viking1.enabled)
        {
            Viking1.enabled = false;
            Viking2.enabled = true;
        }
        else if (Viking2.enabled)
        {
            Viking2.enabled = false;
            Viking1.enabled = true;
        }
    }*/
}
