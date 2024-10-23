using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private float _dodgeSpeed = 5f; // Speed of the dodge
    private bool _isDodging = false; // Flag to check if the enemy is currently dodging;

    [SerializeField]
    private GameObject _enemyLaserPrefab;
    private Transform _playerTransform; // Reference to the player's position

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake;
    private bool _isDestroyed = false;

    public float _increaseWaveSpeed;     // speed that is adjusted based on wave number

    // New speed multiplier
    private float _speedMultiplier = 2f;

    // Updated method for movement to move up instead of down
    private void CalculateMovement()
    {
        
        transform.Translate(Vector3.up * _speed * _speedMultiplier * Time.deltaTime, Space.World);
        
        // Destroy the enemy when it reaches a specific point (if needed)
        if (transform.position.y >= 9f)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _playerTransform = _player.transform;
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        
        _canFire = Time.time + Random.Range(0.2f, 3f); // random delay

        _increaseWaveSpeed = _speed; // set speed to increased speed based on wave

        if (_player == null)
        {
            Debug.LogError("The player is NULL.");
        }

        _explosionAnimation = GetComponent<Animator>();

        if (_explosionAnimation == null)
        {
            Debug.LogError("The explosion animation is NULL.");
        }
    }

    void Update()
    {
        if (_isDestroyed)
        {
            return;
        }

        if (!_isDodging)
        {
            GameObject laser = DetectIncomingLaser();
            if (laser != null)
            {
                PredictiveDodge(laser);
            }
        }

        CalculateMovement();
        

        if (Time.time > _canFire && HasPassedPlayerBy(1.0f))
        {
            FireLasers();
        }
    }

  

    // method to adjust enemy stats based on the wave number
    public void InitializeForWave(int wave)
    {
        // increase enemy speed based on wave number (e.g., 10% increase per wave)
        _increaseWaveSpeed = _speed * _speedMultiplier * (1 + (wave * 0.1f));
    }

    // New method to check if the enemy has passed the player by a certain distance
    private bool HasPassedPlayerBy(float distance)
    {
        // Check if the enemy's Y position is more than the player's Y position plus the specified distance
        return transform.position.y > (_playerTransform.position.y + distance);
    }

    public void FireLasers()
    {
        // Set the fire rate to 1.5 seconds
        _fireRate = 1.5f;
        _canFire = Time.time + _fireRate;

        if (_playerTransform == null) return;

        // Define the offset for the laser behind the enemy
        Vector3 laserOffset = new Vector3(0, -1.9f, 0); // Adjust the Y value to place it behind

        // Instantiate the laser (the laser script will handle direction and movement)
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + laserOffset, Quaternion.identity);
        enemyLaser.GetComponent<SmartEnemyLaser>().SetPlayerTransform(_playerTransform);
    }

    // Detect incoming laser (you can adjust detection logic as needed)
    private GameObject DetectIncomingLaser()
    {
        GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");

        foreach (GameObject laser in lasers)
        {
            float distance = Vector3.Distance(laser.transform.position, transform.position);

            if (distance < 5.0f) // Detect lasers within a certain range
            {
                return laser;
            }
        }

        return null;
    }

    // Predictive dodge based on laser's future position
    private void PredictiveDodge(GameObject laser)
    {
        // Get laser's velocity and predict where it will be in 0.5 seconds
        Vector3 laserVelocity = laser.GetComponent<Rigidbody2D>().velocity;
        Vector3 futurePosition = laser.transform.position + (laserVelocity * 0.5f);  // Adjust 0.5f based on your desired prediction time

        // If the laser's predicted future position is within a danger zone (e.g., 1 unit close on x-axis), dodge
        if (Mathf.Abs(futurePosition.x - transform.position.x) < 1.0f)
        {
            // Debug log to confirm player's laser is triggering the dodge
            Debug.Log("Player's laser detected within danger zone. Enemy is initiating a dodge.");

            // Choose a random dodge direction (left or right) and initiate the smooth dodge
            Vector3 dodgeDirection = Random.value > 0.5f ? Vector3.left : Vector3.right;
            Vector3 dodgeTarget = transform.position + dodgeDirection * 2.0f;  // Adjust dodge distance

            // Start the smooth dodge coroutine
            StartCoroutine(SmoothDodge(dodgeTarget));
        }
    }

    // Smooth dodge movement using interpolation
    private IEnumerator SmoothDodge(Vector3 targetPosition)
    {
        _isDodging = true;  // Set flag to prevent multiple dodges at once

        float elapsedTime = 0;
        Vector3 startPos = transform.position;

        // Calculate the dodge duration based on dodge speed (faster speed = shorter duration)
        float dodgeTime = Vector3.Distance(startPos, targetPosition) / _dodgeSpeed;

        while (elapsedTime < dodgeTime)
        {
            // Interpolate between the start and target position based on elapsed time
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / dodgeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the enemy reaches the exact target position at the end of the dodge
        transform.position = targetPosition;

        _isDodging = false;  // Reset the dodge flag
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

        Destroy(this.gameObject, 1.8f);
    }
}
