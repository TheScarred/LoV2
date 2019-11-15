using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SeleccionPJ : MonoBehaviour
{
    public Image UIImagen;

    public void Start()
    {
        UIImagen = GameObject.Find("ImageCambiante").GetComponent<Image>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UIImagen.sprite = Resources.Load<Sprite>("Sprites/zeropj");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UIImagen.sprite = Resources.Load<Sprite>("Sprites/Knight_idle_01");
        }
    }

    public void FlechitaIzquierda()
    {
        UIImagen.sprite = Resources.Load<Sprite>("Sprites/Knight_idle_01");
    }
    public void FlechitaDerecha()
    {
        UIImagen.sprite = Resources.Load<Sprite>("Sprites/zeropj");
    }
}
