using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDetection : MonoBehaviour
{
    [SerializeField] 
    private float detectionRangeMin = 2f; // minimum detection range
    [SerializeField] 
    private float detectionRangeMax = 8f; // maximum detection range

    private IFireLaser _fireLaserScript; // reference to interface

    private bool _hasFiredLaser = false; // single firing per detection disabled

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
    }

    private void DetectPowerUp()
    {
        // detect all colliders in the detection range in front of enemy
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, detectionRangeMax);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("PowerUp"))
            {
                float distanceToPowerUp = Vector2.Distance(transform.position, hit.transform.position);

                // check if power-up is within specified range
                if (distanceToPowerUp >= detectionRangeMin && distanceToPowerUp <= detectionRangeMax && !_hasFiredLaser)
                {
                    Debug.Log("PowerUp detected!");
                    _fireLaserScript?.FireLasers();  // call FireLasers() in interface
                    _hasFiredLaser = true; // laser fired once

                    // destroy power-up and log the destruction
                    Debug.Log("PowerUp destroyed by enemy!");
                }

                return; // exit method once a power-up is detected and fired at
            }
        }

        _hasFiredLaser = false; // reset if no power-up is in range
    }
}
