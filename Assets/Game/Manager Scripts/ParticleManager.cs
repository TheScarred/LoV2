using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem[] PreFab_Particles;
    List<ParticleSystem> particles;


    public TypesAvailable typesAvailable;
    ParticleSystem particle_created;
     
    void Start()
    {
        List<ParticleSystem> particles = new List<ParticleSystem>();
    }

    public void ActivateParticle(Transform t, TypesAvailable.particleType type)
    {
        print("EnterActivateParticle");
        print("Does it exist?");
        print("List Exists: " + particles.Count);
       
        if (particles.Count > 0)
        {
            print("particles list is not empty");
            for (int i = 0; i < particles.Count; i++)  //search the particle list
            {
                if (particles[i].GetComponent<ParticleType>().particleType == type)
                {
                    if (!particles[i].IsAlive())  //if the particle is not currently being used then...
                    {
                        //play particle
                        Debug.Log("particle found! Playing it!");
                        particles[i].Play();

                        return;
                    }
                }
            }
        }
        Debug.Log("Not found!");
        //if it is not in the list, create a particle from the prefab array
        for (int i = 0; i < PreFab_Particles.Length; i++)  //search the prefab array
        {
            if (PreFab_Particles[i].GetComponent<ParticleType>().particleType == type)  //if we find the type we need in the array
            {
                //create new instance of the particle
                particle_created = Instantiate(PreFab_Particles[i], t);
                print("Particle created: " + particle_created.GetType());
                particles.Add(particle_created);
                print("In List:" + particles[i].GetType());
                particle_created.Play();
            }
        }
    }

    //for testing
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            print("N Pressed");
            TypesAvailable.particleType type = TypesAvailable.particleType.ENEMY_DEATH;
            ActivateParticle(this.transform, type);
        }
    }
}
