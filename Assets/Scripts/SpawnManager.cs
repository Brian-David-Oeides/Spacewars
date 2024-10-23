using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Player _player; // reference to Player
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
    private GameObject _smartEnemyPrefab;

    [SerializeField]
    private float _xOffsetFromPlayer = 5.0f; // Adjustable distance on the x-axis from the player
    [SerializeField]
    private float _xMin = -9f; // Minimum X boundary
    [SerializeField]
    private float _xMax = 9f;  // Maximum X boundary

    [SerializeField]
    private GameObject[] _powerUps;

    private bool _stopSpawning = false;

    private bool _horizontalEnemyActive = false; 

    private int _enemiesSpawned = 0;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }
    }

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

            

            switch (Random.Range(0, 6))
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
                case 5: // New case for SmartEnemy
                    /* if (_player != null)
                    {
                        // Determine the player's movement direction
                        float playerDirectionX = _player.GetPlayerDirectionX();

                        // Spawn the SmartEnemy on the opposite side of player's movement
                        float spawnPosX = _player.transform.position.x + (_xOffsetFromPlayer * -Mathf.Sign(playerDirectionX));

                        // Clamp the x-position to keep it within the scene boundaries
                        spawnPosX = Mathf.Clamp(spawnPosX, _xMin, _xMax);
                        Quaternion rotationToFaceUp = Quaternion.Euler(0, 0, 180);  // This aligns the enemy to face up
                        Vector3 smartEnemySpawnPos = new Vector3(Random.Range(-8f, 8f), -6.85f, 0); // Spawn at fixed Y position
                        GameObject smartEnemy = Instantiate(_smartEnemyPrefab, smartEnemySpawnPos, rotationToFaceUp);
                        smartEnemy.transform.parent = _enemyContainer.transform;
                        Debug.Log("SmartEenmy is Spawning!");
                        Debug.Break();
                    }
                    break;*/
                    Vector3 smartEnemySpawnPos = new Vector3(Random.Range(-9f, 9f), -6.85f, 0); // spawn position
                    GameObject smartEnemy = Instantiate(_smartEnemyPrefab, smartEnemySpawnPos, Quaternion.identity);
                    smartEnemy.transform.parent = _enemyContainer.transform;
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

            // adjust spawn rates based on player's condition
            if (_player.GetLives() < 2) // low health (less than 2 lives)
            {
                healthSpawnChance = 30;  // increase health spawn chance to 30%
                Debug.Log("Health spawn chance increased to: " + healthSpawnChance);
            }

            if (_player.GetCurrentAmmo() < _player.GetMaxAmmo() * 0.2f) // low ammo (less than 20% of max ammo)
            {
                ammoSpawnChance = 60;  // increase ammo spawn chance to 60%
                Debug.Log("Ammo spawn chance increased to: " + ammoSpawnChance);
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
