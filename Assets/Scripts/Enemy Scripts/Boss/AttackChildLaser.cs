using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChildLaser : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private bool hasLockedOn = false;
    private bool continuesOnPath = false;
    private float speed = 5f;
    private Transform playerTransform;
    private float lockThreshold = 1f; // Distance threshold to stop following the player

    public void Initialize(Vector3 initialPlayerPosition, Transform player)
    {
        targetPosition = initialPlayerPosition; // Lock-on position
        playerTransform = player; // Reference to the player for distance check
    }

    private void Update()
    {
        if (!hasLockedOn)
        {
            // Move towards the initial locked position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Lock onto the position once reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                hasLockedOn = true;
                // Calculate direction to continue moving
                moveDirection = (targetPosition - transform.position).normalized;

                // Automatically transition to continuesOnPath state
                continuesOnPath = true;
            }
        }
        else if (continuesOnPath)
        {
            // Continue in the same direction after reaching the last known position
            transform.position += moveDirection * speed * Time.deltaTime;
        }

        // Destroy the laser when it goes out of the scene bounds
        if (transform.position.y < -6f || transform.position.y > 6f || Mathf.Abs(transform.position.x) > 10f)
        {
            Destroy(gameObject);
        }
    }
}