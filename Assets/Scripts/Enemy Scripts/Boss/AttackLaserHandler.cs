using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaserHandler : ILaserHandler
{
    private GameObject attackLaserPrefab;

    public AttackLaserHandler(GameObject prefab)
    {
        attackLaserPrefab = prefab;
    }

    public void FireLaser(Transform bossTransform, Transform playerTransform)
    {
        if (attackLaserPrefab == null || playerTransform == null)
        {
            Debug.LogError("AttackLaserPrefab is null! Cannot fire laser.");
            return;
        }
        
        GameObject laser = GameObject.Instantiate(attackLaserPrefab, bossTransform.position, Quaternion.identity);
        //laser.transform.SetParent(bossTransform); // Make laser a child of the Boss
        laser.SetActive(true); // Enable the laser immediately
        Debug.Log($"Boss position during laser instantiation: {bossTransform.position}");
        //Debug.Break();

        AttackLaser attackLaser = laser.GetComponent<AttackLaser>();
        if (attackLaser != null)
        {
            attackLaser.Initialize(playerTransform.position, -5.8f);
        }
        else
        {
            Debug.LogError("AttackLaserPrefab or PlayerTransform is null! Cannot fire laser.");
        }
        
    }
}
