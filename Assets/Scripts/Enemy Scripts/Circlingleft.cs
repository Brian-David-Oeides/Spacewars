using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circlingleft : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake;
    private bool _isDestroyed = false; 

    
    [SerializeField]
    private float _circleRadius = 3.5f; // Radius of the circle
    private bool _isCircling = false;
    private Vector3 _startPosition;
    private float _circleAngle = 0f; // Angle to calculate circle position
    private float _angularSpeed; // Angular speed for consistent movement
    private Vector3 _circleCenter; // Center of the circle
    private bool _circleCompleted = false;  // Counter for completed circles

    private void Start()
    { 
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();

        // set initial fire time to avoid immediate firing before spawn
        _canFire = Time.time + Random.Range(0.2f, 3f); // random delay

        if (_player == null)
        {
            Debug.LogError("The player is NULL.");
        }

        _explosionAnimation = GetComponent<Animator>();

        if (_explosionAnimation == null)
        {
            Debug.LogError("The player is NULL.");
        }

        // set angular speed based on the linear speed 
        _angularSpeed = _speed * 2.5f;
    }

    void Update()
    {
        if (_isDestroyed)
        {
            return;
        }

        CalculateMovement();
        
        if (Time.time > _canFire)
        {
            FireLasers();
        }
    }

    private void CalculateMovement()
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
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);
        }

        if (_circleCompleted)
        {
            // Move down the Y-axis once the circle is completed
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);

            // If the enemy moves out of bounds (y < -5), destroy the object
            if (transform.position.y < -5f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual void FireLasers()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, this.transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                if (_cameraShake != null)
                {
                    _cameraShake.TriggerShake(0.1f, 0.2f);
                }
            }

            TriggerEnemyDeath();
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            TriggerEnemyDeath();
        }

    }
    // custom method for enemy death
    private void TriggerEnemyDeath()
    {
        // call method for enemy death
        _explosionAnimation.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isDestroyed = true;

        // Disable the collider immediately upon enemy death
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // inform WaveManager enemy was destroyed
        WaveManager waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();

        if (waveManager != null)
        {
            waveManager.EnemyDestroyed(); // notify WaveManager about enemy's destruction
        }
        else
        {
            Debug.LogError("WaveManager is NULL or not found!");
        }

        Destroy(this.gameObject, 2.8f);
    }
}