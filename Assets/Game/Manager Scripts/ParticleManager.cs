using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem[] PreFab_Particles;
    public List<ParticleSystem> particles;

    public TypesAvailable typesAvailable;
    ParticleSystem particle_created;

    private static ParticleManager instance;
    public static ParticleManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ActivateParticle(Transform t, TypesAvailable.particleType type)
    {
        if (particles.Count > 0)
        {
            if (particles[0] == null)
            {
                particles.Clear();
            }
            for (int i = 0; i < particles.Count; i++)  //search the particle list
            {
                if (particles[i].GetComponent<ParticleType>().particleType == type)
                {
                    if (!particles[i].IsAlive())  //if the particle is not currently being used then...
                    {
                        //play particle
                        particles[i].transform.parent = t;
                        particles[i].transform.localPosition = new Vector3(0, 0, 0);
                        particles[i].Play();
                        if (particle_created.main.loop == true)
                        {
                            //Start Timer for Particle
                            particles[i].GetComponent<ParticleType>().StartTimer();
                        }
                        return;
                    }
                }
            }
        }

        //if it is not in the list, create a particle from the prefab array
        for (int i = 0; i < PreFab_Particles.Length; i++)  //search the prefab array
        {
            if (PreFab_Particles[i].GetComponent<ParticleType>().particleType == type)  //if we find the type we need in the array
            {
                //create new instance of the particle + add it to the list
                particle_created = Instantiate(PreFab_Particles[i], t);
                //print("Particle created: " + particle_created.GetType()); 
                
                particles.Add(particle_created);
                particle_created.Play();

                //if the animation is in a loop, call function that will calculate when it needs to be stopped
                if(particle_created.main.loop == true)
                {
                    particle_created.GetComponent<ParticleType>().StartTimer();
                }
                return;
            }
        }
    }

    private void OnApplicationQuit()
    {
        particles.Clear();
    }
    
    //for testing
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            //print("N Pressed");
            TypesAvailable.particleType type = TypesAvailable.particleType.ENEMY_DEATH;
            ActivateParticle(this.transform, type);
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            //print("M Pressed");
            TypesAvailable.particleType type = TypesAvailable.particleType.MOD_DEFENSE;
            ActivateParticle(this.transform, type);
        }
    }
}
