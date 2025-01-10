using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeLaser : MonoBehaviour
{
    private Vector3 _targetPosition;       // Player's locked position
    private float _speed = 8f;            // Movement speed
    private float _sineAmplitude = 1f;    // Amplitude of sine wave motion
    private float _sineFrequency = 2f;    // Frequency of sine wave motion
    private float _time;                  // Tracks elapsed time for sine wave
   

    public void Initialize(Vector3 playerPosition, float destroyBoundary)
    {
        _targetPosition = playerPosition;
    }

    private void Update()
    {
        MoveInSineWaveTowardsTarget();
    }

    private void MoveInSineWaveTowardsTarget()
    {
        // Calculate the direction toward the target
        Vector3 direction = (_targetPosition - transform.position).normalized;

        // Sine wave offset perpendicular to the direction of travel
        float sineOffset = Mathf.Sin(_time * _sineFrequency) * _sineAmplitude;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * sineOffset;

        // Combine forward motion with sine wave offset
        Vector3 movement = direction * _speed * Time.deltaTime + perpendicular;

        transform.position += movement;

        // Update time for sine wave calculation
        _time += Time.deltaTime;
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
