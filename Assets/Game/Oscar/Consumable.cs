using UnityEngine;

public class Consumable : MonoBehaviour
{
    public int id;

    void Start()
    {
        ParticleManager.GetInstance().ActivateParticle(transform, TypesAvailable.particleType.CONSUMABLE);
    }
}


