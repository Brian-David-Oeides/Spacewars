using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaser : MonoBehaviour
{
    private Vector3 _targetPosition;       // The position the laser will aim for initially
    private Vector3 _exitPosition;        // The position the laser will head toward after passing the target
    private float _speed = 8f;            // Speed of the laser
    private bool _isTargetLocked = true;  // Tracks if the laser is still moving toward the target

    // Initializes the laser with its target, exit position, and destruction boundary
    public void Initialize(Vector3 targetPosition, Vector3 exitPosition, float destroyBoundary)
    {
        _targetPosition = targetPosition;
        _exitPosition = exitPosition;
    }

    private void Update()
    {
        MoveLaser();
    }

    private void MoveLaser()
    {
        // Determine the current direction: toward the target or exit
        Vector3 currentTarget = _isTargetLocked ? _targetPosition : _exitPosition;

        // Move the laser toward the current target
        Vector3 direction = (currentTarget - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        // Check if the laser has reached the target
        if (_isTargetLocked && Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _isTargetLocked = false; // Switch to moving toward the exit position
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
                player.Damage(1); // Inflict damage to the player
            }

            Destroy(gameObject); // Destroy the laser on collision
        }
    }
}

