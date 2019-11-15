using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{


    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("ENEMIES"))
        {
            this.gameObject.SetActive(false);
        }
    }

}
