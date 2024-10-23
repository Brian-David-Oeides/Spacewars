using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f; // Speed at which the laser moves
    private Vector3 _direction;   // Direction in which the laser will move
    private Transform _playerTransform; // Reference to the player's position
    private ShakeCamera _cameraShake;


    public void SetPlayerTransform(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        SetDirection(_playerTransform.position - transform.position);
    }

    void Start()
    {
        // Find the ShakeCamera component on the main camera
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
    }

    void Update()
    {
        MoveLaser();
    }

    // Method to set the direction of the laser
    private void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;

        // Rotate the laser to face the direction without affecting its scale
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Method to move the laser
    private void MoveLaser()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

        // Destroy the laser when it goes off-screen
        if (transform.position.y > 8.0f || transform.position.y < -8.0f ||
            transform.position.x > 11.0f || transform.position.x < -11.0f)
        {
            Destroy(this.gameObject);
        }
    }

    // Handle collisions with the player or other lasers
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Damage the player
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                if (_cameraShake != null)
                {
                    _cameraShake.TriggerShake(0.1f, 0.2f);
                }
            }
            // Destroy the laser
            Destroy(this.gameObject);
        }    
    }
}
