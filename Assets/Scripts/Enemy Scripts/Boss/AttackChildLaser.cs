using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChildLaser : MonoBehaviour
{
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;
    private bool _hasLockedOn = false;
    private bool _continuesOnPath = false;
    private float _speed = 5f;
    private Transform _playerTransform;

    public void Initialize(Vector3 initialPlayerPosition, Transform player)
    {
        _targetPosition = initialPlayerPosition; // Lock-on position
        _playerTransform = player; // Reference to the player for distance check
    }

    private void Update()
    {
        if (!_hasLockedOn)
        {
            // Move towards the initial locked position
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

            // Lock onto the position once reached
            if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
            {
                _hasLockedOn = true;
                // Calculate direction to continue moving
                _moveDirection = (_targetPosition - transform.position).normalized;

                // Automatically transition to continuesOnPath state
                _continuesOnPath = true;
            }
        }
        else if (_continuesOnPath)
        {
            // Continue in the same direction after reaching the last known position
            transform.position += _moveDirection * _speed * Time.deltaTime;
        }

        // Destroy the laser when it goes out of the scene bounds
        if (transform.position.y < -11f || Mathf.Abs(transform.position.x) > 11f)
        {
            Debug.Log($"{gameObject.name} destroyed at position: {transform.position}");
            // Check if the parent exists and destroy it
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
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