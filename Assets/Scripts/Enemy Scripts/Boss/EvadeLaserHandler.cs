using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeLaserHandler : ILaserHandler
{
    private GameObject evadeLaserPrefab;

    public EvadeLaserHandler(GameObject prefab)
    {
        evadeLaserPrefab = prefab;
    }

    public void FireLaser(Transform bossTransform, Transform playerTransform)
    {
        if (evadeLaserPrefab != null && playerTransform != null)
        {
            // Instantiate the parent laser object
            GameObject laserParent = GameObject.Instantiate(evadeLaserPrefab, bossTransform.position, Quaternion.identity);

            // Access child lasers
            Transform laserLeft = laserParent.transform.Find("Laser_Left");
            Transform laserRight = laserParent.transform.Find("Laser_Right");

            if (laserLeft != null)
            {
                EvadeChildLaser leftLaser = laserLeft.GetComponent<EvadeChildLaser>();
                if (leftLaser != null)
                {
                    // Initialize the left laser with the player's position
                    leftLaser.Initialize(playerTransform.position + new Vector3(-2f, 0, 0), playerTransform);
                }
            }
            else
            {
                Debug.LogError("Laser_Left not found in EvadeLaser prefab!");
            }

            if (laserRight != null)
            {
                EvadeChildLaser rightLaser = laserRight.GetComponent<EvadeChildLaser>();
                if (rightLaser != null)
                {
                    // Initialize the right laser with the player's position
                    rightLaser.Initialize(playerTransform.position + new Vector3(2f, 0, 0), playerTransform);
                }
            }
            else
            {
                Debug.LogError("Laser_Right not found in EvadeLaser prefab!");
            }
        }
        else
        {
            Debug.LogError("EvadeLaser prefab or playerTransform is null!");
        }
    }
}
