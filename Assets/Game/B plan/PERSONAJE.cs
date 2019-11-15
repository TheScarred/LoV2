using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PERSONAJE : MonoBehaviour
{

    int speed = 5;
   
    

    //public arma arma; 


    // Start is called before the first frame update
   

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("ENEMIES"))
        {
            //AQUI LE BAJA LA VIDA

            //PERO PONDRE DE EJEMPLO QUE SOLO LO DESHABILITE
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovimientoJugador();   
        if(Input.GetKeyDown(KeyCode.Space))
        {

            //arma.isFiring = true;

             
            //go.transform.position = pos;
        }
        else
        {
            //arma.isFiring = false;
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
