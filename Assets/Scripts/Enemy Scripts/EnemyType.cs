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
}
