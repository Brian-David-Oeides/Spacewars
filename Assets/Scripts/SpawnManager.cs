using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // declare game object to instantiate
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
   
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        
    }

    // Spawn game objects every 5 seconds
    IEnumerator SpawnRoutine()
    {
        // while loop
        while (true)
        {
            // define the position for he enemy prefab
            Vector3 positionToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            // Instantiate enemy Prefab
            GameObject newEnemy = Instantiate(_enemyPrefab,positionToSpawn, Quaternion.identity);
            // yield wait for 5 seconds
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f); // delay 5 frames then call next line
        }  
    }
}
