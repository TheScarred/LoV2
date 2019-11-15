using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // ARMAS BASE DE LOS CUALES SE BASARA EL PROGRAMA CUANDO CREE A SUS HIJOS
    public GameObject prefabLegendaryWeapon;
    public GameObject prefabEpicWeapon;

    // EN EL CASO DE LAS ARMAS NORMALES Y RARAS ESTAS SE GUARDARAN EN UNA LISTA CON EL FIN DE APAGARSE Y VOLVER A PRENDERSE CUANDO SEA NECESARIO EN LUGAR DE ELIMINARLOS Y VOLVER A CREARLOS
    public GameObject prefabRareWeapon;
    List<GameObject> poolRareWeapons = new List<GameObject>();
    public GameObject prefabWeapon;
    List<GameObject> poolWeapons = new List<GameObject>();

    // FUNCION ENCARGADA DE DEFINIR SI UN ARMA ES CREADA DEL MONSTRUO DESTRUIDO
    public void ItemDropRate(Vector3 _spawnPosition)
    {
        int createItem = Random.Range(1, 100);

        if (createItem > 70)
        {
            Debug.Log(createItem);
            ItemDropRateByRarity(_spawnPosition);
        }


    }

    // FUNCION ENCARGADA DE DEFINIR LA RAREZA DE DICHA ARMA UNA VEZ SE HAYA DEFINIDO QUE ESTA SI DEBE SER CREADA
    void ItemDropRateByRarity(Vector3 _spawnPosition)
    {
        int createRarity = Random.Range(1, 100);

        Debug.Log(createRarity);
        if (createRarity > 95)
        {
            Debug.Log("Legendaria");
            //SpawnLegendaryWeapon(_spawnPosition);
        }
        else if (createRarity > 80)
        {
            Debug.Log("Epica");
            //SpawnEpicWeapon(_spawnPosition);
        }
        else if (createRarity > 60)
        {
            Debug.Log("Rara");
            //SpawnRareWeapon(_spawnPosition);
        }
        else
        {
            Debug.Log("Normal");
            //SpawnWeapon(_spawnPosition);
        }
    }

    // FUNCIONES ENCARGADAS DE SPAWNEAR EL ARMA TOMANDO EN CUENTA SU RAREZA
    #region SpawnDeArmas
    GameObject SpawnLegendaryWeapon(Vector3 _spawnPosition)
    {
        GameObject actualWeapon = Instantiate(prefabLegendaryWeapon);
        return actualWeapon;
    }

    GameObject SpawnEpicWeapon(Vector3 _spawnPosition)
    {
        GameObject actualWeapon = Instantiate(prefabEpicWeapon);
        return actualWeapon;
    }

    GameObject SpawnRareWeapon(Vector3 _spawnPosition)
    {
        for (int i = 0; i < poolWeapons.Count; i++)
        {
            if (poolWeapons[i].activeSelf == false)
            {
                poolRareWeapons[i].SetActive(true);
                return poolWeapons[i];
            }
        }

        GameObject actualWeapon = Instantiate(prefabRareWeapon);
        actualWeapon.name = actualWeapon.name + "_" + poolWeapons.Count;
        poolRareWeapons.Add(actualWeapon);
        return actualWeapon;
    }

    GameObject SpawnWeapon(Vector3 _spawnPosition)
    {
        for (int i = 0; i < poolWeapons.Count; i++)
        {
            if (poolWeapons[i].activeSelf == false)
            {
                poolWeapons[i].SetActive(true);
                return poolWeapons[i];
            }
        }

        GameObject actualWeapon = Instantiate(prefabWeapon);
        actualWeapon.name = actualWeapon.name + "_" + poolWeapons.Count;
        poolWeapons.Add(actualWeapon);
        return actualWeapon;
    }
    #endregion
}
