using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingEnemy : Enemy
{
    private float _circleRadius = 3.5f; // radius of the circle
    private bool _isCircling = true;
    private Vector3 _startPosition;
    private float _circleAngle = 0f; // Angle to calculate circle position

    protected override void Start()
    {
        base.Start();
        _startPosition = transform.position; // Set the starting position for circling

    }

    protected override void CalculateMovement()
    {
        if (_isCircling)
        {
            // Calculate the circular motion
            _circleAngle -= _speed * Time.deltaTime;
            float xOffset = Mathf.Cos(_circleAngle) * _circleRadius;
            float yOffset = Mathf.Sin(_circleAngle) * _circleRadius;

            // Update position for circular movement
            Vector3 currentPosition = new Vector3(_startPosition.x + xOffset, _startPosition.y + yOffset, 0);
            transform.position = currentPosition;

            Vector3 direction = currentPosition - _startPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Check if the circle is completed (once 360 degrees is covered)
            if (_circleAngle <= -2 * Mathf.PI) // 2 * PI radians = 360 degrees
            {
                _circleAngle = 0f; // Reset the angle
                _isCircling = false;
            }
        }
        else
        {
            // Move down the Y-axis once the circle is completed
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            // If the enemy moves out of bounds (y < -5), destroy the object
            if (transform.position.y < -5f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}

