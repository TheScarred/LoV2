using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class InputEnemigo : PunBehaviour
{
    private Transform jugador;
    public float vertical { get { return direccionHaciaJugador.y; } }
    public float horizontal { get { return direccionHaciaJugador.x; } }
    public float distancia { get { return direccionHaciaJugador.magnitude; } }
    private Vector2 direccionHaciaJugador;
    // Start is called before the first frame update
    void Start()
    {
        jugador = PhotonConnection.GetInstance().ownPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        direccionHaciaJugador = jugador.position = transform.transform.position;
    }
}
