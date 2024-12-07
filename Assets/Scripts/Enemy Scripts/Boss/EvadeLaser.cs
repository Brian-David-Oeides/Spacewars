using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeLaser : MonoBehaviour
{
    private Vector3 targetPosition;       // Player's locked position
    private float speed = 8f;            // Movement speed
    private float sineAmplitude = 1f;    // Amplitude of sine wave motion
    private float sineFrequency = 2f;    // Frequency of sine wave motion
    private float time;                  // Tracks elapsed time for sine wave
    private float destroyBoundary = -6f; // Destruction boundary on the y-axis

    public void Initialize(Vector3 playerPosition, float destroyBoundary)
    {
        targetPosition = playerPosition;
        this.destroyBoundary = destroyBoundary;
    }

    private void Update()
    {
        MoveInSineWaveTowardsTarget();
        CheckDestructionBoundary();
    }

    private void MoveInSineWaveTowardsTarget()
    {
        // Calculate the direction toward the target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Sine wave offset perpendicular to the direction of travel
        float sineOffset = Mathf.Sin(time * sineFrequency) * sineAmplitude;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * sineOffset;

        // Combine forward motion with sine wave offset
        Vector3 movement = direction * speed * Time.deltaTime + perpendicular;

        transform.position += movement;

        // Update time for sine wave calculation
        time += Time.deltaTime;
    }

    private void CheckDestructionBoundary()
    {
        // Destroy the laser when it exits the screen bounds
        if (transform.position.y <= destroyBoundary || Mathf.Abs(transform.position.x) > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision with the player
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(1); // Apply damage to the player
            }

            Destroy(gameObject); // Destroy the laser on collision
        }
    }
}
