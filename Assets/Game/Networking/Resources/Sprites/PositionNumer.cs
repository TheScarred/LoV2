using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PositionNumer : MonoBehaviour
{
    // Start is called before the first frame update
    public Image IndicadorLugar;

    void Start()
    {
        IndicadorLugar = GameObject.Find("ImagenDrewsin").GetComponent<Image>();
    }

    // Update is called once per frame
    public void PrimerLugar()
    {
        IndicadorLugar.sprite = Resources.Load<Sprite>("Sprites/PrimerLugar");
    }
    public void SegundoLugar()
    {
        IndicadorLugar.sprite = Resources.Load<Sprite>("Sprites/SegundoLugar");

    }
    public void TercerLugar()
    {
        IndicadorLugar.sprite = Resources.Load<Sprite>("Sprites/TercerLugar");
    }
    public void CuartoLugar()
    {
        IndicadorLugar.sprite = Resources.Load<Sprite>("Sprites/CuartoLugar");
    }
}
