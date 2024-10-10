using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _horizontalEnemyPrefab;
    [SerializeField]
    private GameObject _chasingEnemyPrefab;
    [SerializeField]
    private GameObject _sideToSideEnemyPrefab;
    [SerializeField]
    private GameObject _circlingRightEnemyPrefab;
    [SerializeField]
    private GameObject _circlingLeftEnemyPrefab;
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

            switch (Random.Range(0, 5))
            {
                case 0:
                    GameObject sideToSideEnemy = Instantiate(_sideToSideEnemyPrefab, positionToSpawn, Quaternion.identity);
                    sideToSideEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case 1:
                    GameObject chasingEnemy = Instantiate(_chasingEnemyPrefab, positionToSpawn, Quaternion.identity);
                    chasingEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case 2:
                    GameObject circlingRightEnemy = Instantiate(_circlingRightEnemyPrefab, positionToSpawn, Quaternion.identity);
                    circlingRightEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case 3:
                    GameObject circlingLeftEnemy = Instantiate(_circlingLeftEnemyPrefab, positionToSpawn, Quaternion.identity);
                    circlingLeftEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case 4: // Instantiate the HorizontalEnemy
                    Vector3 horizontalEnemySpawnPos = new Vector3(-14.85f, Random.Range(5f, 7f), 0); // Spawning offscreen on the left
                    GameObject horizontalEnemy = Instantiate(_horizontalEnemyPrefab, horizontalEnemySpawnPos, Quaternion.identity);
                    horizontalEnemy.transform.parent = _enemyContainer.transform;
                    break;
                default:
                    Debug.LogError("Base Enemy ");
                    break;
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
            else if (randomValue < 10) // adjust for 10%
            {
                randomPowerUpIndex = 6; //disable fire laser power-up
            }
            else if (randomValue < 35) 
            {
                randomPowerUpIndex = Random.Range(0, 5); 
            }
            else 
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
