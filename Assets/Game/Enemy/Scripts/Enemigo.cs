using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemigo : MonoBehaviour
{
    public int vida;
    NavMeshAgent NavMesh;
    public Transform objetivo;

    // Start is called before the first frame update
    void Start()
    {
        NavMesh = GetComponent<NavMeshAgent>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullets"))
        {
            vida = vida - 1;
        }
        else return;
    }

    void Muerte()
    {
        if (vida <= 0)
        {
            vida = 0;
            this.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        Muerte();
        NavMesh.SetDestination(objetivo.position);
    }
}
