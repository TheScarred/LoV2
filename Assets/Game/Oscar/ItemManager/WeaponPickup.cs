using UnityEngine;
using Items;

public class WeaponPickup : MonoBehaviour
{
    public WeaponType type;
    public WeaponRarity rarity;
    public int ID;

    public WeaponPickup()
    {
        /*this.ID = PhotonConnection.GetInstance().WeaponID;
        PhotonConnection.GetInstance().WeaponID++;
        PhotonConnection.GetInstance().weaponList.Add(this);*/
    }
}
