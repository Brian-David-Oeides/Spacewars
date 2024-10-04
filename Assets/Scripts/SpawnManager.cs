using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] _enemyPrefab; //add array
    [SerializeField] 
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerUps;

    private bool _stopSpawning = false;

    public void StartSpawning(int enemyCount, int wave) // add enemy count and wave number
    {
        StartCoroutine(SpawnEnemyRoutine(enemyCount, wave)); 
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine(int enemyCount, int wave) 
    {
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < enemyCount; i++)
        {
            if (_stopSpawning) yield break;

            Vector3 positionToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab[0], positionToSpawn, Quaternion.identity); //[0]
            newEnemy.transform.parent = _enemyContainer.transform;

            int randomEnemyType = Random.Range(0, 4);

            switch (randomEnemyType)
            {
                case 0:
                    SideToSideEnemy sideToSideEnemy = newEnemy.AddComponent<SideToSideEnemy>();
                    sideToSideEnemy.SetLaserPrefab(_enemyLaserPrefab);
                    break;
                case 1: 
                    ChasingEnemy chasingEnemy = newEnemy.AddComponent<ChasingEnemy>();
                    chasingEnemy.SetLaserPrefab(_enemyLaserPrefab);
                    break;
                case 2:
                    CirclingEnemy circlingEnemy = newEnemy.AddComponent<CirclingEnemy>();
                    circlingEnemy.SetLaserPrefab(_enemyLaserPrefab);
                    break;
                case 3:
                    Circlingleft circlingLeft = newEnemy.AddComponent<Circlingleft>();
                    circlingLeft.SetLaserPrefab(_enemyLaserPrefab);
                    break;
                default:
                    Debug.LogError("default enemy type");
                    break;
            }
            
            // customize enemy stats for current wave (e.g., speed scaling)
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();

            if (enemyComponent != null)
            {
                enemyComponent.InitializeForWave(wave); // adjust stats based on wave
            }

            yield return new WaitForSeconds(3.0f); 
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 positionToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomValue = Random.Range(0, 100); 
            int randomPowerUpIndex; 

            if (randomValue < 5) 
            {
                randomPowerUpIndex = 5; 
            }
            else if (randomValue < 35) 
            {
                randomPowerUpIndex = Random.Range(0, 5); 
            }
            else // or else
            {
                randomPowerUpIndex = Random.Range(0, _powerUps.Length);
            }

            Instantiate(_powerUps[randomPowerUpIndex], positionToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    // reset to allow spawning again if needed (for example, after restart)
    public void ResetSpawning()
    {
        _stopSpawning = false; // enable spawning
    }

}
