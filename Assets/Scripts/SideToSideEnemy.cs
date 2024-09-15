using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSideEnemy : Enemy
{
    // lateral movement boundaries
    private float _amplitude = 2.0f;

    // lateral movement speed (affects how fast the enemy moves side to side)
    private float _frequency = 6.0f;

    // time tracker for the cosine function
    private float _timeCounter = 0.0f;


    protected override void CalculateMovement()
    { 
        // increment based on real-time * frequency
        _timeCounter += Time.deltaTime * _frequency;
        // calculate X position based on cosine function 
        float x = Mathf.Cos(_timeCounter) * _amplitude;

        // calculate downward movement 
        float downwardSpeed = _speed * 0.25f; // slow down _speed = 1
        float newY = this.transform.position.y - downwardSpeed * Time.deltaTime;

        // set new position for enemy (oscillating X movement + downward Y movement)
        this.transform.position = new Vector3(x, newY, this.transform.position.z);

        // Log position for debugging
        Debug.Log("SideToSideEnemy position: " + transform.position);
    }
}
