using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSideEnemy : Enemy
{
    // lateral movement boundaries
    private float _limitedLateralMovement = 3.0f;
    
    // lateral direction
    private float _lateralDirection = 2.0f;

    protected override void CalculateMovement()
    {
        transform.Translate(_lateralDirection * _speed * Time.deltaTime * Vector3.right);

        if (transform.position.x > _limitedLateralMovement || transform.position.x < -_limitedLateralMovement)
        {
            _lateralDirection *= -1;
        }

        float slowerDownwardSpeed = _speed * 0.5f; // slower downward speed
        // Continue moving downwards
        transform.Translate(slowerDownwardSpeed * Time.deltaTime * Vector3.down);
    }
}
