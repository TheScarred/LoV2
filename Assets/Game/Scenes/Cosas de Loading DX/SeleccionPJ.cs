using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SeleccionPJ : MonoBehaviour
{
    public Image Viking1;
    public Image Viking2;

    public void Start()
    {
        Viking2.enabled = false;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //UIImagen.sprite = Resources.Load<Sprite>("Sprites/zeropj");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
           //UIImagen.sprite = Resources.Load<Sprite>("Sprites/Knight_idle_01");
        }
    }

    public void Arrow()
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
    }
}
