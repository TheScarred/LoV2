using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Jugador : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    public GameObject[] Armas;
    

    GameObject prefabMelee;
    GameObject prefabRango;
    int speed = 5;
    public Vector3 posicionJugador;



  

    // Update is called once per frame
    void Update()
    {
        //MovimientoJugador();
        posicionJugador = transform.position;
        //Debug.Log(posicionJugador);

    }


    void OnTriggerEnter(Collider objeto)
    {
        if (objeto.CompareTag("Melee"))
        {
            Debug.Log("Presiona p para agarrar ");

        }

        else if (objeto.CompareTag("Rango"))
        {
            Debug.Log("Presiona p para agarrar ");

        }

    }

    void OnTriggerStay(Collider objeto)
    {
        if (objeto.CompareTag("Melee"))
        {
            if (Input.GetKeyDown(KeyCode.P) && Armas[0] == null)
            {
                Armas[0] = objeto.gameObject;
                prefabMelee = Armas[0];
                prefabMelee.gameObject.SetActive(false);
                prefabMelee.GetComponent<Weapons>().enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.P) && Armas[0] != null)
            {
                prefabMelee.GetComponent<Weapons>().enabled = true;
                objeto.gameObject.SetActive(false);
                Armas[0] = objeto.gameObject;
                prefabMelee.SetActive(true);

                prefabMelee = Armas[0];


            }
        }
       
        else if (objeto.CompareTag("Rango"))
        {
            if (Input.GetKeyDown(KeyCode.P) && Armas[1] == null)
            {
                Armas[1] = objeto.gameObject;
                prefabRango = Armas[1];
                objeto.gameObject.SetActive(false);
                prefabRango.GetComponent<Weapons>().enabled = true;

            }
            else if (Input.GetKeyDown(KeyCode.P) && Armas[1] != null)
            {
                prefabRango.GetComponent<Weapons>().enabled = true;
                objeto.gameObject.SetActive(false);
                Armas[1] = objeto.gameObject;
                prefabRango.gameObject.SetActive(true);

                prefabRango = Armas[1];
            }

        }

        
    }

   
  

    void MovimientoJugador()
    {
      if (Input.GetKey(KeyCode.S))
      {
          transform.Translate(Vector3.back * speed * Time.deltaTime);
      }
      else if (Input.GetKey(KeyCode.W))
      {
          transform.Translate(Vector3.forward * speed * Time.deltaTime);
      }

      if (Input.GetKey(KeyCode.A))
      {
          transform.Translate(Vector3.left * speed * Time.deltaTime);
      }
      else if (Input.GetKey(KeyCode.D))
      {
          transform.Translate(Vector3.right * speed * Time.deltaTime);
      }  
    }
}
