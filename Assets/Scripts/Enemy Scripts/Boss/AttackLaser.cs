using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaser : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 exitPosition;
    private float speed = 8f;
    private bool isLockingOn = true; // Controls the two-stage movement

    public void Initialize(Vector3 target, Vector3 exit, float destroyBoundary)
    {
        targetPosition = target;
        exitPosition = exit;
    }

    private void Update()
    {
        Vector3 direction;
        if (isLockingOn)
        {
            // Move towards the player's position
            direction = (targetPosition - transform.position).normalized;
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isLockingOn = false; // Switch to moving towards the exit position
            }
        }
        else
        {
            // Move towards the exit position
            direction = (exitPosition - transform.position).normalized;
        }

        transform.position += direction * speed * Time.deltaTime;

        // Destroy the laser when it moves past the destroy boundary
        if (Mathf.Abs(transform.position.y) > 10f || Mathf.Abs(transform.position.x) > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(1);
            }
            Destroy(gameObject);
        }
    }
}

