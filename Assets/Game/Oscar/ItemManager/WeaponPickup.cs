using UnityEngine;
using Items;

public class WeaponPickup : MonoBehaviour
{
    public WeaponType type;
    public WeaponRarity rarity;
    public int ID;
    public int lastWear;

    void Start()
    {
        ParticleManager.GetInstance().ActivateParticle(transform, TypesAvailable.particleType.CONSUMABLE);
    }
}
