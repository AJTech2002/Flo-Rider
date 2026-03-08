using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnInterval = 0.5f; 
    
    void Start()
    {
        if (prefabToSpawn != null)
            StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (GameManager.Instance.isGameRunning())
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPrefab()
    {
        int xRandom = Random.Range(-6, 6) * 2;
        int zRandom = Random.Range(-6, 6) * 2;

        if (xRandom == 0 && zRandom == 0) return; // Tower
        
        Vector3 spawnPosition = new Vector3(
            xRandom,
            0,
            zRandom
        );
        
        

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
