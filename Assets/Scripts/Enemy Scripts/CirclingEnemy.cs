using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingEnemy : Enemy
{
    private float _circleRadius = 3.5f; // radius of the circle
    private bool _isCircling = false; // Start by moving downward
    private bool _circleCompleted = false;  // Counter for completed circles
    private Vector3 _circleCenter; // Center of the circle
    private float _circleAngle = 0f; // Angle to calculate circle position

    protected override void Start()
    {
        base.Start();

        _canBaseFire = false; // prevent base class from firing

    }

    protected override void CalculateMovement()
    {

        if (!_isCircling && transform.position.y <= 5.5f && !_circleCompleted)
        {
            // at y = 5.5f, start circling
            _isCircling = true;

            // offset center of circle's radius to left (along the -x-axis)
            _circleCenter = new Vector3(transform.position.x - _circleRadius, transform.position.y, 0);
        }

        if (_isCircling)
        {
            // calculate circular motion
            _circleAngle -= _speed * Time.deltaTime;
            float xOffset = Mathf.Cos(_circleAngle) * _circleRadius;
            float yOffset = Mathf.Sin(_circleAngle) * _circleRadius;

            // update position for circular movement
            Vector3 currentPosition = new Vector3(_circleCenter.x + xOffset, _circleCenter.y + yOffset, 0);
            transform.position = currentPosition;

            // rotate sprite to match movement direction
            Vector3 direction = currentPosition - _circleCenter;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // check if circle is completed 
            if (_circleAngle <= -2 * Mathf.PI)
            {
                _circleAngle = 0f;   // reset the angle
                _isCircling = false;
                _circleCompleted = true;  // mark circle as completed
            }
        }
        else if (!_circleCompleted)
        {
            // continue moving downward once circling is done
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        // once the circle is done, continue downward movement normally
        if (_circleCompleted)
        {
            // continue moving down 
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            // destroy the object if it goes out of bounds
            if (transform.position.y < -5f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}

