using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Player jugador;
    public GameObject yo;
    Vector3 posicionJugador;


    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Debug.Log(jugador.posicionJugador);

        yo.transform.position =  jugador.posicionJugador;
        Debug.Log(yo.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //posicionJugador = jugador.posicionJugador;
        
        //posicionJugador = transform.localPosition;

    }
}
