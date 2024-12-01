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
            // Spawn left laser
            Vector3 leftPosition = bossTransform.position + new Vector3(-1f, 0f, 0f);
            GameObject leftLaser = GameObject.Instantiate(evadeLaserPrefab, leftPosition, Quaternion.identity);
            EvadeLaser leftLaserComponent = leftLaser.GetComponent<EvadeLaser>();
            if (leftLaserComponent != null)
            {
                Vector3 arcTarget = playerTransform.position + new Vector3(-2f, 0, 0); // Adjust as needed for arc
                leftLaserComponent.Initialize(arcTarget, -5.8f);
            }

            // Spawn right laser
            Vector3 rightPosition = bossTransform.position + new Vector3(1f, 0f, 0f);
            GameObject rightLaser = GameObject.Instantiate(evadeLaserPrefab, rightPosition, Quaternion.identity);
            EvadeLaser rightLaserComponent = rightLaser.GetComponent<EvadeLaser>();
            if (rightLaserComponent != null)
            {
                Vector3 arcTarget = playerTransform.position + new Vector3(2f, 0, 0); // Adjust as needed for arc
                rightLaserComponent.Initialize(arcTarget, -5.8f);
            }
        }
    }
}
