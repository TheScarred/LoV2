using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SeleccionPJ : MonoBehaviour
{
    public Image Viking1;
    public Image Viking2;
    public Savenick save;

    public void Start()
    {
        Viking2.enabled = false;
    }

    public void Arrow()
    {
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

        save.GuardarPJ();
        save.AsignarPJ();
    }
}
