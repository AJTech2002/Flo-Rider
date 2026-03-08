using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int numberOfCars = 20;
    public float avoidanceRadius = 5.0f;
    
    public void StartSpawning()
    {
        int spawnedCars = 0;
        while (spawnedCars < numberOfCars)
        {
            if (SpawnPrefab())
            {
                spawnedCars++;
            }
        }
    }
  
    bool SpawnPrefab()
    {
        int xRandom = Random.Range(-5, 5) * 2;
        int zRandom = Random.Range(-5, 5) * 2;

        if (xRandom == 0 && zRandom == 0) return false;
        
        Vector3 spawnPosition = new Vector3(
            xRandom,
            0,
            zRandom
        );
        
        // Do a raycast down and check for grass tag
        RaycastHit hit;
        Ray ray = new Ray(spawnPosition + Vector3.up * 10, Vector3.down);
        if (Physics.Raycast(ray, out hit, 20))
        {
            if (hit.collider.gameObject.tag == "Grass" || hit.collider.gameObject.tag == "Car")
            {
                return false;
            }
        }
        
        // Check for other cars in radius
        foreach (var car in GameObject.FindObjectsOfType<CarMovement>())
        {
            if (Vector3.Distance(car.transform.position, spawnPosition) < avoidanceRadius)
            {
                return false;
            }
        }
        
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        return true;
    }
}
