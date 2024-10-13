using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private float _speed = 5f; // speed 
    [SerializeField] private float _homingRange = 3f; // range to home onto player
    private float _selfDestructY = -4.7f; // Y coordinate where missile will self-destruct
    private Transform _playerTransform; // reference to player's transform
    private bool _isHoming = true; // check if missile is homing
    private Vector3 _lastDirection; // last direction missile was traveling

    private ShakeCamera _cameraShake;

    void Start()
    {
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();

        // find player's transform
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player GameObject is NULL.");
        }
    }

    void Update()
    {
        if (_playerTransform != null && _isHoming)
        {
            // calculate distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _homingRange)
            {
                // store last direction before disabling homing
                _lastDirection = (_playerTransform.position - transform.position).normalized;

                // disable homing
                _isHoming = false;
            }

            else
            {
                // continue homing towards player
                _lastDirection = (_playerTransform.position - transform.position).normalized;
            }
        }

        // move in the last known direction (either homing or after homing stops)
        transform.Translate(_lastDirection * _speed * Time.deltaTime);

        // destroy if it reaches the y-coordinate of -4.7
        if (transform.position.y <= _selfDestructY)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == ("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(); // access player Damage() method

                if (_cameraShake != null)
                {
                    StartCoroutine(_cameraShake.Shake(0.3f, 0.5f));
                }
            }

            Destroy(this.gameObject, 0.3f); // Destroy missile on collision with player
        }
    }
}