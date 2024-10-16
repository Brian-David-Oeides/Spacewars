using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Player player; // reference to Player
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

    private bool _horizontalEnemyActive = false; 

    private int _enemiesSpawned = 0;

    public void StartSpawning(int enemyCount, int wave) 
    {
        StartCoroutine(SpawnEnemyRoutine(enemyCount, wave));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine(int enemyCount, int wave)
    {
        yield return new WaitForSeconds(3.0f);

        _enemiesSpawned = 0;

        while (!_stopSpawning)
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
                case 4: // instantiate HorizontalEnemy
                    if (!_horizontalEnemyActive) // check flag before instantiating
                    {
                        Vector3 horizontalEnemySpawnPos = new Vector3(-14.85f, Random.Range(5f, 7f), 0);
                        GameObject horizontalEnemy = Instantiate(_horizontalEnemyPrefab, horizontalEnemySpawnPos, Quaternion.identity);
                        horizontalEnemy.transform.parent = _enemyContainer.transform;
                        _horizontalEnemyActive = true; // set flag to true
                    }
                    break;
                default:
                    Debug.LogError("Base Enemy ");
                    break;
            }

            _enemiesSpawned++;

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

            int healthSpawnChance = 10; // 10% chance (rare) Health
            int ammoSpawnChance = 40; // 40% chance (frequent) Ammo

            // Adjust spawn rates based on player's condition
            if (player._lives < 2) // Low health (less than 2 lives)
            {
                healthSpawnChance = 30;  // Increase health spawn chance to 30%
            }
            if (player._currentAmmo < player._maxAmmo * 0.2f) // Low ammo (less than 20% of max ammo)
            {
                ammoSpawnChance = 60;  // Increase ammo spawn chance to 60%
            }

            // adust spawn rate 
            if (randomValue < healthSpawnChance) // 10% chance (rare)
            {
                randomPowerUpIndex = 4; // Health - Element 4
            }
            else if (randomValue < healthSpawnChance + ammoSpawnChance) // 40% chance 
            {
                randomPowerUpIndex = 3; //Ammo - Element 3
            }
            else if (randomValue < 70) // 20% chance
            {
                randomPowerUpIndex = Random.Range(0, 2); // Speed, TripleShot, Shield - Element 0 -2
            }
            else if (randomValue < 80) // 10% chance
            {
                randomPowerUpIndex = Random.Range(5, 7); // MultiShot, EMPE, Smart Missile - Element 5-7
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

    public void ResetSpawning()
    {
        _stopSpawning = false; 
    }

    public void OnHorizontalEnemyDestroyed()
    {
        _horizontalEnemyActive = false; // reset flag when Horizontal Enemy is destroyed
    }
}
