using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrains", menuName = " Data/Terreno", order = 1)]
public class TerrainGenerator : ScriptableObject
{

    public GameObject[] terrenos;
    public List<GameObject> EnemySpawners;
    public List<GameObject> PlayerSpawners;
    public Vector3 startPos;
    public Vector3 advancePosX;
    public Vector3 advancePosY;
    public int numberOfTerrains;
    public int maxNumberX;

    public void SetTerrain(int _numberOfTerrains, Transform _parent)
    {
        EnemySpawners.Clear();
        EnemySpawners = new List<GameObject>();
        PlayerSpawners.Clear();
        PlayerSpawners = new List<GameObject>();
        if (terrenos.Length < _numberOfTerrains)
        {
            Debug.LogError("NO HAY SUFICIENTES SECTORES");
            return;
        }
        int numX = 0;
        int numY = 0;
        int[] terrainsUsed = new int[_numberOfTerrains];


        for (int j = 0; j < terrainsUsed.Length; j++)
        {
            terrainsUsed[j] = -1;
        }

        for (int x = 0; x < _numberOfTerrains; x++)
        {
            int randomNum = Random.Range(0, terrenos.Length);
            //bool newNumber = false;
            //  int counter = 15+_numberOfTerrains; //medida deseguriddad para salirde los ciclos
            //while (!newNumber)
            //{
            for (int j = 0; j < terrainsUsed.Length; j++)
            {
                if (terrainsUsed[j] < 0)
                {
                    terrainsUsed[j] = randomNum;
                    //newNumber = true;
                    break;
                }
                else
                {
                    if (terrainsUsed[j] == randomNum)
                    {
                        randomNum = Random.Range(0, terrenos.Length);
                        break;
                    }
                }
            }

            GameObject g = Instantiate(terrenos[randomNum], _parent);
            g.transform.position = startPos + (advancePosX * numX) + (advancePosY * numY);
            for (int p = 0; p < g.GetComponent<SpawnerEnemies>().EnemySpawners.Count; p++)
            {
                EnemySpawners.Add(g.GetComponent<SpawnerEnemies>().EnemySpawners[p].gameObject);
            }

            for (int p = 0; p < g.GetComponent<SpawnerEnemies>().PlayerSpawners.Count; p++)
            {
                PlayerSpawners.Add(g.GetComponent<SpawnerEnemies>().PlayerSpawners[p].gameObject);
            }


            numX++;
            if (numX >= maxNumberX)
            {
                numX = 0;
                numY++;


            }

            //}

        }
       

    }


}
