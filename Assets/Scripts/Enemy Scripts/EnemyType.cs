using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "ScriptableObjects/EnemyType")]

public class EnemyType : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public float speed;
    public int health;
    public GameObject enemyLaserPrefab;
    public bool canReceiveShield = true; // control shield application

    // New fields for unique enemy types
    public bool requiresUniqueSpawnPosition = false;
    public Vector3 uniqueSpawnPosition;
    public bool requiresRotation = false;
    public Quaternion enemyRotation;
    public bool isHorizontalEnemy = false;
}
