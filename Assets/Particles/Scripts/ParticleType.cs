using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleType : MonoBehaviour
{
    public TypesAvailable.particleType particleType;
    public float base_countdown;
    float countdown;
    public ParticleSystem particle;

    //Timer to turn off looping particle

    private void Awake()
    {
        countdown = base_countdown;
    }
    public void StartTimer()
    {
        StartCoroutine(timer());
    }
    IEnumerator timer()
    {
        yield return new WaitForSeconds(countdown);
        particle.Stop();
        //print("Stop!");
        countdown = 3;
    }
}
