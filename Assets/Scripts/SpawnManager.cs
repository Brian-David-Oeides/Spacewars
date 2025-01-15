using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private Player _player;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField] 
    private GameObject _enemyShieldPrefab;

    [SerializeField] 
    private WaveManager _waveManager;

    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField] 
    private EnemyType[] enemyTypes;  // Replaces individual prefab fields
    [SerializeField] 
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    private bool _stopEnemySpawning = false;  // Controls enemy spawning (set when Boss spawns)

    private bool _horizontalEnemyActive = false;

    private bool _bossSpawned = false; // track Boss spawning

    //private int _enemiesSpawned = 0;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }
        else
        {
            playerTransform = _player.transform; // Assign the player's transform
        }

        _waveManager = FindObjectOfType<WaveManager>();

        if (_waveManager == null)
        {
            Debug.LogError("WaveManager not found in the scene!");
        }
    }

    /// <summary>
    /// Starts the enemy and power-up spawning routines.
    /// </summary>
    public void StartSpawning(int enemyCount, int wave) 
    {
        StartCoroutine(SpawnEnemyRoutine(enemyCount, wave));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    /// <summary>
    /// Spawns enemies using object pooling instead of direct instantiation.
    /// </summary>
    IEnumerator SpawnEnemyRoutine(int enemyCount, int wave)
    {
        yield return new WaitForSeconds(3.0f);

        int _enemiesSpawned = 0;

        while (!_stopSpawning && _waveManager.GetEnemiesDestroyed() < enemyCount)
        {
            // Select a random enemy type
            EnemyType selectedEnemyType = enemyTypes[Random.Range(0, enemyTypes.Length)];

            // Skip HorizontalEnemy if already active
            if (selectedEnemyType.isHorizontalEnemy && _horizontalEnemyActive)
            {
                yield return null;
                continue;
            }

            Vector3 positionToSpawn = selectedEnemyType.requiresUniqueSpawnPosition
                ? selectedEnemyType.uniqueSpawnPosition
                : new Vector3(Random.Range(-8f, 8f), 7, 0);

            Quaternion spawnRotation = selectedEnemyType.requiresRotation
                ? selectedEnemyType.enemyRotation
                : Quaternion.identity;

            // Using Object Pool instead of Instantiating
            GameObject spawnedEnemy = EnemyPoolManager.Instance.GetEnemy(selectedEnemyType, positionToSpawn, spawnRotation);
            spawnedEnemy.transform.parent = _enemyContainer.transform;

            // Apply properties and logic
            Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(selectedEnemyType.speed, selectedEnemyType.enemyLaserPrefab, selectedEnemyType);
                enemyScript.SetWaveManagerReference(FindObjectOfType<WaveManager>());
            }

            // Apply shield if needed
            ApplyShield(spawnedEnemy, selectedEnemyType);

            // Track horizontal enemy
            if (selectedEnemyType.isHorizontalEnemy)
            {
                _horizontalEnemyActive = true;
            }

            _enemiesSpawned++;
            yield return new WaitForSeconds(3.0f);
        }

        Debug.Log("Finished spawning all enemies for this wave.");
    }

    /// <summary>
    /// Applies a shield to an enemy if the ScriptableObject specifies it.
    /// </summary>
    private void ApplyShield(GameObject enemy, EnemyType enemyType)
    {
        // Only apply shield if the enemyType allows it
        if (!enemyType.canReceiveShield) return;

        // random chance to apply a shield to enemy type
        if (Random.value > 0.5f) // adjust probability as needed
        {
            GameObject shield = Instantiate(_enemyShieldPrefab, enemy.transform.position, Quaternion.identity);
            shield.transform.SetParent(enemy.transform); // set shield as child of the enemy
            shield.SetActive(true); // activate shield animation

            EnemyShield shieldComponent = enemy.GetComponentInChildren<EnemyShield>(); // get shield class component
            // if shield is active and the random value is greater than 0.5
            if (shieldComponent != null)
            {
                // activate shield & debug.log
                shieldComponent.Activate();
                Debug.Log("Enemy shield activated from spawn manager!");
            }
        }
    }

    public int GetTotalEnemiesToSpawn()
    {
        int count = 0;
        foreach (EnemyType enemy in enemyTypes)
        {
            if (!enemy.isHorizontalEnemy || !_horizontalEnemyActive)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns an enemy to the object pool instead of destroying it.
    /// </summary>
    public void ReturnEnemyToPool(EnemyType enemyType, GameObject enemy)
    {
        EnemyPoolManager.Instance.ReturnEnemy(enemyType, enemy);
    }

    /// <summary>
    /// Power-up spawning routine using regular instantiation logic.
    /// </summary>
    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
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
        _stopEnemySpawning = true;
    }

    public void ResetSpawning()
    {
        _stopSpawning = false;
        _stopEnemySpawning = false;
    }

    public void OnHorizontalEnemyDestroyed()
    {
        _horizontalEnemyActive = false; 
    }
    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    /// <summary>
    /// Spawns the boss without object pooling, as it is a unique event.
    /// </summary>
    public void SpawnBoss()
    {
        if (_bossSpawned) return; // Prevent multiple Boss spawns

        _bossSpawned = true; // Set Boss spawn flag
        _stopEnemySpawning = true; // Stop enemy spawning
        // Do not set _stopSpawning, so power-ups continue to spawn

        Vector3 bossSpawnPosition = new Vector3(0, 7, 0); // Spawn Boss at top-center
        GameObject boss = Instantiate(_bossPrefab, bossSpawnPosition, Quaternion.identity);

        Boss bossScript = boss.GetComponent<Boss>();
        if (bossScript != null)
        {
            bossScript.Initialize(playerTransform, _bossPrefab.GetComponent<Boss>().attackLaserPrefab, _bossPrefab.GetComponent<Boss>().evadeLaserPrefab);
        }
        else
        {
            Debug.LogError("Boss script not found on the Boss prefab!");
        }

        Debug.Log("Boss spawned!");
    }

    public void StopPowerUpSpawning() // New method to stop power-ups
    {
        _stopSpawning = true;
    }
}
