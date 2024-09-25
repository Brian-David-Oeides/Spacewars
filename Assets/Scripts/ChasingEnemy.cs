using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : Enemy
{
    private Transform _playerTransform;
    private float _angle;
    private bool _isFallingBack = false;  // new flag to control state

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
            Vector3 directionToPlayer = _playerTransform.position - this.transform.position;

            // calculate angle towards the player
            _angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));

            // calculate distance to player
            float distanceToPlayer = Vector3.Distance(this.transform.position, _playerTransform.position);

            // if within 6.0f of the player, fallback
            if (distanceToPlayer < 6.0f)
            {
                _isFallingBack = true;  // enable fallback mode, stop chasing
            }
            else
            {
                // move towards the player
                Vector3 moveDirection = directionToPlayer.normalized;
                this.transform.Translate(moveDirection * _speed * Time.deltaTime, Space.World);
            }
        }

        if (_isFallingBack)  // enter fallback mode
        {
            // move down the y-axis
            this.transform.Translate(Vector3.down * _speed * Time.deltaTime);

            // if position goes below -4.6f, destroy or reset
            if (this.transform.position.y < -4.6f)
            {
                // rotate towards the negative y-axis
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // face downwards

                Destroy(this.gameObject);  // or reset position depending on behavior needed
                Debug.Log("Chasing enemy self-destructed.");
            }
        }
    }
}
