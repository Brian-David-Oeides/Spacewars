using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : Enemy
{
    private Transform _playerTransform;
    private float _angle;
    private bool _isFallingBack = false;  // New flag to control state

    protected override void Start()
    {
        base.Start();
        if (_player != null)
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }

    protected override void CalculateMovement()
    {
        if (_playerTransform != null && !_isFallingBack)
        {
            Vector3 directionToPlayer = _playerTransform.position - transform.position;

            // Calculate angle towards the player
            _angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));

            // Calculate distance to player
            float distanceToPlayer = Vector3.Distance(this.transform.position, _playerTransform.position);

            // If within 3.0f of the player, move away
            if (distanceToPlayer < 4.0f)
            {
                _isFallingBack = true;  // Set fallback mode, stop chasing
            }
            else
            {
                // Move towards the player
                Vector3 moveDirection = directionToPlayer.normalized;
                transform.Translate(moveDirection * _speed * Time.deltaTime, Space.World);
            }
        }

        if (_isFallingBack)  // Now only execute this once we enter fallback mode
        {
            // Move down the y-axis
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            // If position goes below -5.0f, destroy or reset
            if (transform.position.y < -5.0f)
            {
                Destroy(this.gameObject);  // Or reset position depending on behavior needed
            }
        }
    }
}
