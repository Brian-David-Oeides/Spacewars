using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Player _player; 
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
    private GameObject _enemyShieldPrefab; // Reference to the shield prefab

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

            int enemyType = Random.Range(0, 6);
            
            GameObject spawnedEnemy = null;

            switch (enemyType) 
            {
                case 0:
                    spawnedEnemy = Instantiate(_sideToSideEnemyPrefab, positionToSpawn, Quaternion.identity);
                    break;
                case 1:
                    spawnedEnemy = Instantiate(_chasingEnemyPrefab, positionToSpawn, Quaternion.identity);
                    break;
                case 2:
                    spawnedEnemy = Instantiate(_circlingRightEnemyPrefab, positionToSpawn, Quaternion.identity);
                    break;
                case 3:
                    spawnedEnemy = Instantiate(_circlingLeftEnemyPrefab, positionToSpawn, Quaternion.identity);
                    break;
                case 4: 
                    if (!_horizontalEnemyActive)
                    {
                        Vector3 horizontalEnemySpawnPos = new Vector3(-14.85f, Random.Range(5f, 7f), 0);
                        GameObject horizontalEnemy = Instantiate(_horizontalEnemyPrefab, horizontalEnemySpawnPos, Quaternion.identity);
                        horizontalEnemy.transform.parent = _enemyContainer.transform;
                        _horizontalEnemyActive = true; 
                    }
                    break;
                case 5: 
                    Vector3 smartEnemySpawnPos = new Vector3(Random.Range(-9f, 9f), -6.85f, 0); 
                    Quaternion smartEnemyRotation = Quaternion.Euler(0, 0, 180); 
                    GameObject smartEnemy = Instantiate(_smartEnemyPrefab, smartEnemySpawnPos, smartEnemyRotation);
                    smartEnemy.transform.parent = _enemyContainer.transform;
                    break;
                default:
                    Debug.LogError("Base Enemy");
                    break;
            }

            // apply shield if enemy was spawned
            if (spawnedEnemy != null)
            {
                ApplyShield(spawnedEnemy);
                spawnedEnemy.transform.parent = _enemyContainer.transform; // organize in a container
            }

            _enemiesSpawned++;

            yield return new WaitForSeconds(3.0f);
        }
    }

    // ApplyShield method
    private void ApplyShield(GameObject enemy)
    {
        // random chance to apply a shield to enemy type
        if (Random.value > 0.5f) // adjust probability as needed
        {
            GameObject shield = Instantiate(_enemyShieldPrefab, enemy.transform.position, Quaternion.identity);
            shield.transform.SetParent(enemy.transform); // set shield as child of the enemy
            shield.SetActive(true); // activate shield animation

            EnemyShield shieldComponent = shield.GetComponent<EnemyShield>(); // get shield class component
            // if shield is active and the random value is greater than 0.5
            if (shieldComponent != null)
            {
                // activate shield & debug.log
                shieldComponent.Activate();
                Debug.Log("Enemy shield activated from spawn manager!");
            }
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

            int healthSpawnChance = 10; 
            int ammoSpawnChance = 40; 

            if (_player.GetLives() < 2) 
            {
                healthSpawnChance = 30;  
                Debug.Log("Health spawn chance increased to: " + healthSpawnChance);
            }

            if (_player.GetCurrentAmmo() < _player.GetMaxAmmo() * 0.2f) 
            {
                ammoSpawnChance = 60;  
                Debug.Log("Ammo spawn chance increased to: " + ammoSpawnChance);
            }

            if (randomValue < healthSpawnChance) 
            {
                randomPowerUpIndex = 4; 
            }
            else if (randomValue < healthSpawnChance + ammoSpawnChance) 
            {
                randomPowerUpIndex = 3; 
            }
            else if (randomValue < 70) 
            {
                randomPowerUpIndex = Random.Range(0, 2); 
            }
            else if (randomValue < 80) 
            {
                randomPowerUpIndex = Random.Range(5, 7); 
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
        _horizontalEnemyActive = false; 
    }
}
