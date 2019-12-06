using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] positionsOfRespawn;
    public GameObject[] enemies;
    public Vector3 spawnValues;
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;
    public bool stop;
    public int enemiesSpawned;
    public int maxEnemies;

    
    int randEnemy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitSpawner());
    }

    // Update is called once per frame
    void Update()
    {
            spawnWait = Random.Range(spawnLeastWait, spawnMostWait);
    }
    IEnumerator waitSpawner()
    {
        yield return new WaitForSeconds(startWait);
        while (!stop)
        {
            randEnemy = Random.Range(0, 3);
            Vector3 spawnPosition = new Vector3 (Random.Range(-spawnValues.x, spawnValues.x),0.5f,Random.Range (-spawnValues.z, spawnValues.z));
            Instantiate(enemies[0],spawnPosition+transform.TransformPoint(0,0,0),gameObject.transform.rotation);

            //Instantiate(enemies[0], positionsOfRespawn[Random.Range(0, positionsOfRespawn.Length)].position + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
            yield return new WaitForSeconds(spawnWait);
        }
    }
}
