using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : Enemy
{
    private Transform _playerTransform;
    private float _offsetDistance = 3.0f; // Adjust this value for how much offset you want
    //private float _moveSpeed = 3.0f; // You can change this for faster or slower movement
    private float _triggerDistance = 4.0f; // Distance at which the enemy moves past the player
    private float _pastPositionY = -5f; // Target Y position to move past the player

    // Override the base class movement calculation
    protected override void CalculateMovement()
    {
        if (_player == null)
        {
            // Find the player if not already assigned
            _player = GameObject.Find("Player").GetComponent<Player>();
        }

        if (_player != null)
        {
            _playerTransform = _player.transform;

            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // If the enemy is within the trigger distance, move it past the player to a position with y < -5f
            if (distanceToPlayer <= _triggerDistance)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, _pastPositionY, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            }
            else
            {
                // Calculate the direction towards the player
                Vector3 direction = (_playerTransform.position - transform.position).normalized;

                // Offset the movement direction to avoid direct collision
                Vector3 offsetDirection = direction * _offsetDistance;

                // Move the enemy towards the offset position near the player
                transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position + offsetDirection, _speed * Time.deltaTime);
            }
        }

        // Reset enemy position if it goes off-screen
        if (transform.position.y < _pastPositionY)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }
}
