using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TerrainGenerator terrainsToGenerate;
    public List<GameObject> EnemySpawners;
    public int numOfTerrains;
    // Start is called before the first frame update
    void Start()
    {
        terrainsToGenerate.SetTerrain(numOfTerrains, this.transform);
    }

    public void CreateSpawnPoints()
    {
        EnemySpawners = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {

                if (transform.GetChild(i).transform.GetChild(j).CompareTag("Spawner Enemies"))
                {
                    EnemySpawners.Add(transform.GetChild(i).transform.GetChild(j).gameObject);
                }
            }

        }
    }
}
