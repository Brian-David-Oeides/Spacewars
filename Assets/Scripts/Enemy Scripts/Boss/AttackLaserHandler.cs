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
        if (attackLaserPrefab != null && playerTransform != null)
        {
            // Instantiate the parent laser object
            GameObject laserParent = GameObject.Instantiate(attackLaserPrefab, bossTransform.position, Quaternion.identity);

            // Access child lasers
            Transform laserLeft = laserParent.transform.Find("Laser_Left");
            Transform laserRight = laserParent.transform.Find("Laser_Right");

            if (laserLeft != null)
            {
                AttackLaser leftLaser = laserLeft.GetComponent<AttackLaser>();
                if (leftLaser != null)
                {
                    // Set the target position and exit position
                    Vector3 leftExitPosition = playerTransform.position + new Vector3(-2f, -10f, 0f);
                    leftLaser.Initialize(playerTransform.position, leftExitPosition, -5.8f);
                }
            }

            if (laserRight != null)
            {
                AttackLaser rightLaser = laserRight.GetComponent<AttackLaser>();
                if (rightLaser != null)
                {
                    // Set the target position and exit position
                    Vector3 rightExitPosition = playerTransform.position + new Vector3(2f, -10f, 0f);
                    rightLaser.Initialize(playerTransform.position, rightExitPosition, -5.8f);
                }
            }
        }
    }
}
