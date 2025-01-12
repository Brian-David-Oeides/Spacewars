using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    [SerializeField] private EnemyType[] enemyTypes;
    private Dictionary<EnemyType, Queue<GameObject>> enemyPools = new Dictionary<EnemyType, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (EnemyType enemyType in enemyTypes)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < 10; i++)  // Pool size of 10 per enemy type
            {
                GameObject enemy = Instantiate(enemyType.enemyPrefab);
                enemy.SetActive(false);
                pool.Enqueue(enemy);
            }
            enemyPools.Add(enemyType, pool);
        }
    }

    public GameObject GetEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyPools.ContainsKey(enemyType) && enemyPools[enemyType].Count > 0)
        {
            GameObject enemy = enemyPools[enemyType].Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);

            // Apply ScriptableObject properties
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(enemyType.speed, enemyType.enemyLaserPrefab);
            }
            return enemy;
        }
        else
        {
            // Expand pool if empty
            GameObject newEnemy = Instantiate(enemyType.enemyPrefab, position, rotation);
            return newEnemy;
        }
    }

    public void ReturnEnemy(EnemyType enemyType, GameObject enemy)
    {
        enemy.SetActive(false);
        if (!enemyPools.ContainsKey(enemyType))
        {
            enemyPools[enemyType] = new Queue<GameObject>();
        }
        enemyPools[enemyType].Enqueue(enemy);
    }
}
