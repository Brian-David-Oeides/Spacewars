using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeChildLaser : MonoBehaviour
{
    private Vector3 _targetPosition; // The player's locked position
    private Vector3 _moveDirection; // Direction to continue moving
    private bool _continuesOnPath = false;
    private float _speed = 5f; // Vertical speed
    private float _sineAmplitude = 0.2f; // Amplitude of sine wave
    private float _sineFrequency = 20f; // Frequency of sine wave
    private Transform _playerTransform;
    private float _lockThreshold = 1f; // Distance threshold to stop following the player
    private float _time; // Tracks elapsed time for sine wave

    public void Initialize(Vector3 initialPlayerPosition, Transform player)
    {
        _targetPosition = initialPlayerPosition; // Lock-on position
        _playerTransform = player; // Reference to the player for distance check
    }

    private void Update()
    {
        _time += Time.deltaTime;

        // Calculate sine wave offset
        float sineOffset = Mathf.Sin(_time * _sineFrequency) * _sineAmplitude;

        // Calculate movement direction toward the target
        Vector3 direction = (_targetPosition - transform.position).normalized;

        // If the laser is within lockThreshold, transition to continuous movement
        if (Vector3.Distance(transform.position, _targetPosition) <= _lockThreshold && !_continuesOnPath)
        {
            _continuesOnPath = true;
            _moveDirection = direction; // Lock direction for continuous movement
        }

        // Combine directional movement with sine wave
        Vector3 sineWaveMovement = new Vector3(sineOffset, 0, 0);
        if (_continuesOnPath)
        {
            // Continue moving in the last known direction with sine wave
            transform.position += _moveDirection * _speed * Time.deltaTime + sineWaveMovement;
        }
        else
        {
            // Move towards the target with sine wave
            transform.position += direction * _speed * Time.deltaTime + sineWaveMovement;
        }

        // Destroy the laser when it goes out of the scene bounds
        if (transform.position.y < -6f || Mathf.Abs(transform.position.x) > 11f)
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
