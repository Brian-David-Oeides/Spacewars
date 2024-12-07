using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeChildLaser : MonoBehaviour
{
    private Vector3 targetPosition; // The player's locked position
    private Vector3 moveDirection; // Direction to continue moving
    private bool hasLockedOn = false;
    private bool continuesOnPath = false;
    private float speed = 5f; // Vertical speed
    private float sineAmplitude = 0.10f; // Amplitude of sine wave
    private float sineFrequency = 20f; // Frequency of sine wave
    private Transform playerTransform;
    private float lockThreshold = 1f; // Distance threshold to stop following the player
    private float time; // Tracks elapsed time for sine wave

    public void Initialize(Vector3 initialPlayerPosition, Transform player)
    {
        targetPosition = initialPlayerPosition; // Lock-on position
        playerTransform = player; // Reference to the player for distance check
    }

    private void Update()
    {
        time += Time.deltaTime;

        // Calculate sine wave offset
        float sineOffset = Mathf.Sin(time * sineFrequency) * sineAmplitude;

        // Calculate movement direction toward the target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // If the laser is within lockThreshold, transition to continuous movement
        if (Vector3.Distance(transform.position, targetPosition) <= lockThreshold && !continuesOnPath)
        {
            continuesOnPath = true;
            moveDirection = direction; // Lock direction for continuous movement
        }

        // Combine directional movement with sine wave
        Vector3 sineWaveMovement = new Vector3(sineOffset, 0, 0);
        if (continuesOnPath)
        {
            // Continue moving in the last known direction with sine wave
            transform.position += moveDirection * speed * Time.deltaTime + sineWaveMovement;
        }
        else
        {
            // Move towards the target with sine wave
            transform.position += direction * speed * Time.deltaTime + sineWaveMovement;
        }

        // Destroy the laser when it goes out of the scene bounds
        if (transform.position.y < -6f || transform.position.y > 6f || Mathf.Abs(transform.position.x) > 10f)
        {
            // Check if the parent exists and destroy it
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    // Ensure this part remains unchanged for collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(1); // Inflict damage
            }
            Destroy(gameObject); // Destroy on collision
        }
    }
}
