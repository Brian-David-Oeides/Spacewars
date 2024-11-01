using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDetection : MonoBehaviour
{
    [SerializeField] private float detectionRangeMin = 2f;
    [SerializeField] private float detectionRangeMax = 8f;

    private IFireLaser _fireLaserScript;

    private bool _hasFiredLaser = false; // single firing per detection

    void Start()
    {
        _fireLaserScript = GetComponent<IFireLaser>();

        if (_fireLaserScript == null)
        {
            Debug.LogError("No script implementing IFireLaser found on this GameObject.");
        }
    }
    void Update()
    {
        DetectPowerUp();
        Debug.Log("PowerUp detected!");
    }

    private void DetectPowerUp()
    {
        // Detect all colliders in the detection range in front of the enemy
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, detectionRangeMax);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("PowerUp"))
            {
                float distanceToPowerUp = Vector2.Distance(transform.position, hit.transform.position);

                // Check if the power-up is within the specified range
                if (distanceToPowerUp >= detectionRangeMin && distanceToPowerUp <= detectionRangeMax && !_hasFiredLaser)
                {
                    _fireLaserScript?.FireLasers();  // Call FireLasers() on the interface

                    _hasFiredLaser = true; // Laser fired once
                }

                return; // Exit the method once a power-up is detected and fired at
            }
        }

        _hasFiredLaser = false; // Reset if no power-up is in range
    }
}
