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
    private float sineAmplitude = 1f; // Amplitude of sine wave
    private float sineFrequency = 2f; // Frequency of sine wave
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
                continuesOnPath = true; // Automatically transition to path continuation
            }
        }
        else if (continuesOnPath)
        {
            // Move in a sine wave motion while moving downward
            float sineOffset = Mathf.Sin(time * sineFrequency) * sineAmplitude;
            Vector3 sineWaveMovement = new Vector3(sineOffset, -speed * Time.deltaTime, 0);

            // Continue moving in the sine wave pattern
            transform.position += moveDirection * Time.deltaTime * speed + sineWaveMovement;
        }

        // Destroy the laser when it goes out of the scene bounds
        if (transform.position.y < -6f || transform.position.y > 6f || Mathf.Abs(transform.position.x) > 10f)
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
                player.Damage(1); // Inflict damage
            }
            Destroy(gameObject); // Destroy on collision
        }
    }
}
