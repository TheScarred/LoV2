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
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ActivateParticle(Transform t, TypesAvailable.particleType type, bool NeedParent = true)
    {
        if (particles.Count > 0)
        {
           
            for (int i = 0; i < particles.Count; i++)  //search the particle list
            {
                if (particles[i] == null)  //if there is an error in the list where a reference is not found or something is missing
                {
                    particles.Clear();  //will delete the contents of the list
                    return;  //particle will not show but will work next time
                }
                if (particles[i].GetComponent<ParticleType>().particleType == type)
                {
                    if (!particles[i].IsAlive())  //if the particle is not currently being used then...
                    {
                        //check if particle needs to have a parent
                        if (NeedParent)
                        {
                            particles[i].transform.parent = t;
                            particles[i].transform.localPosition = Vector3.zero; 
                        }
                        else
                        {
                            particles[i].transform.position = t.position;
                        }
                        //play particle
                        particles[i].Play();
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
                particle_created = Instantiate(PreFab_Particles[i], t.position, PreFab_Particles[i].transform.rotation);
                particle_created.transform.localPosition = Vector3.zero;

                //check if particle needs a parent
                if (NeedParent)  //if parent is needed add parent
                {
                    particle_created.transform.parent = t;
                    particle_created.transform.localPosition = Vector3.zero;
                }

                
                particles.Add(particle_created);
                particle_created.Play();

                return;
            }
        }
    }

    public void StopParticle(Transform t)
    {
        for (int i = 0; i < particles.Count; i++)
        {
            if (particles[i] == null)  //if there is an error in the list where a reference is not found or something is missing
            {
                particles.Clear();  //will delete the contents of the list
                return;  //particle will not show but will work next time
            }
            if (particles[i].transform.parent == t.gameObject)
            {
                particles[i].transform.parent = null;
                particles[i].Stop();
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
