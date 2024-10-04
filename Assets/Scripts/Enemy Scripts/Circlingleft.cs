using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circlingleft : Enemy
{   
    private float _circleRadius = 3.5f; // Radius of the circle
    private bool _isCircling = false;
    private Vector3 _startPosition;
    private float _circleAngle = 0f; // Angle to calculate circle position
    private float _angularSpeed; // Angular speed for consistent movement
    private Vector3 _circleCenter; // Center of the circle
    private bool _circleCompleted = false;  // Counter for completed circles

    protected override void Start()
    {
        base.Start();
        Debug.Log("Circling Left Enemy Initialized.");

        _canBaseFire = false; // prevent base class from firing

        // set angular speed based on the linear speed 
        _angularSpeed = _speed * 3.5f;
    }

    protected override void CalculateMovement()
    {
        if (!_isCircling && transform.position.y <= 5.5f && !_circleCompleted)
        {
            // at y = 5.5f, start circling
            _isCircling = true;

            // offset center of circle's radius to left (along the -x-axis)
            _circleCenter = new Vector3(transform.position.x + _circleRadius, transform.position.y, 0);
        }

        if (_isCircling)
        {
            // Decrease _circleAngle for counterclockwise (leftward) movement
            _circleAngle -= _angularSpeed * Time.deltaTime / _circleRadius; // Adjusted for consistent speed

            // Calculate the x and y offsets based on the current angle
            float xOffset = -Mathf.Cos(_circleAngle) * _circleRadius;
            float yOffset = Mathf.Sin(_circleAngle) * _circleRadius;

            // Update position for circular movement
            Vector3 currentPosition = new Vector3(_circleCenter.x + xOffset, _circleCenter.y + yOffset, 0);
            transform.position = currentPosition;

            // Calculate the tangent vector (direction of movement)
            float dx = _circleRadius * Mathf.Sin(_circleAngle);
            float dy = _circleRadius * Mathf.Cos(_circleAngle);
            Vector3 direction = new Vector3(dx, dy, 0).normalized;

            // Calculate the angle from the tangent vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Adjust angle based on default orientation
            angle -= 90f; // Subtract 90 degrees if the sprite faces up by default

            // Apply rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Check if the circle is completed (once 360 degrees is covered)
            if (_circleAngle <= -2 * Mathf.PI) // 2 * PI radians = 360 degrees
            {
                _circleAngle = 0f; // Reset the angle
                _isCircling = false;
                _circleCompleted = true;  // mark circle as completed
            }
        }
        else if (!_circleCompleted)
        {
            // continue moving downward once circling is done
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (_circleCompleted)
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