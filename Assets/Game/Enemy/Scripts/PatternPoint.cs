using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPoint : MonoBehaviour
{
    private bool collided = false;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            collided = true;
        }
        else
        {
            collided = false;
        }
    }

    public void GenerateNewPosition(float minX, float maxX, float minY, float maxY)
    {
        transform.position = new Vector3(Random.Range(minX, maxX), 0.25f, Random.Range(minY, maxY));
    }

}
