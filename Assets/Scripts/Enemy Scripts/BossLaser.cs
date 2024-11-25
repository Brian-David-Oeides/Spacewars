using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [SerializeField] private float speed = 8f;       // Speed of the laser
    private Vector3 targetPosition;                 // Target position for the laser
    private bool isArched = false;                  // Whether the laser follows an arched path
    private Vector3 arcOffset;                      // Offset for arched movement
    private float destroyY = -5.8f;                 // Y-axis boundary for self-destruction

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void SetOnMissDestroyY(float y)
    {
        destroyY = y;
    }

    public void SetArchedMovement(Vector3 playerPosition, float destroyY)
    {
        this.destroyY = destroyY;
        isArched = true;
        arcOffset = (transform.position.x < playerPosition.x) ? new Vector3(-2f, -2f, 0) : new Vector3(2f, -2f, 0);
    }

    private void Update()
    {
        if (isArched)
        {
            // Arched movement logic
            Vector3 direction = (targetPosition + arcOffset - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else if (targetPosition != Vector3.zero)
        {
            // Targeted movement logic
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // Default downward movement
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        // Destroy the laser if it goes out of bounds
        if (transform.position.y <= destroyY)
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
                player.Damage(1); // Example damage value
            }

            Destroy(gameObject); // Destroy the laser upon collision
        }
    }
}
