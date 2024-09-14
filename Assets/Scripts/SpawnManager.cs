using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField] 
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerUps;
    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 positionToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, positionToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            int randomEnemyType = Random.Range(0, 2);

            switch (randomEnemyType)
            {
                case 0:
                    // Assign SideToSideEnemy component and set laser prefab
                    newEnemy.AddComponent<SideToSideEnemy>();
                    SideToSideEnemy sideToSideEnemy = newEnemy.GetComponent<SideToSideEnemy>();
                    sideToSideEnemy.SetLaserPrefab(_enemyLaserPrefab);
                    break;
                /*case 1:
                    // Assign CirclingEnemy component and set laser prefab
                    newEnemy.AddComponent<CirclingEnemy>();
                    CirclingEnemy circlingEnemy = newEnemy.GetComponent<CirclingEnemy>();
                    circlingEnemy._enemyLaserPrefab = enemyLaserPrefab;
                    break;

                case 2:
                    // Assign AngledEnemy component and set laser prefab
                    newEnemy.AddComponent<AngledEnemy>();
                    AngledEnemy angledEnemy = newEnemy.GetComponent<AngledEnemy>();
                    angledEnemy._enemyLaserPrefab = enemyLaserPrefab;
                    break;*/
                default:
                    Debug.LogError("Unknown enemy type");
                    break;
            }

            yield return new WaitForSeconds(5.0f); 
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 positionToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomValue = Random.Range(0, 100); //Random value between 0 and 99
            int randomPowerUpIndex; // a new variable stores chance power up is spawned

            if (randomValue < 5) // if random value is less than 5%
            {
                randomPowerUpIndex = 5; // then spawn power up index 5
            }
            else if (randomValue < 35) // or else if random value less than 35%
            {
                randomPowerUpIndex = Random.Range(0, 5); // then spawn power up index 0 to 5
            }
            else // or else
            {
                randomPowerUpIndex = Random.Range(0, _powerUps.Length);// random power up select any power up
            }

            Instantiate(_powerUps[randomPowerUpIndex], positionToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
    
}
