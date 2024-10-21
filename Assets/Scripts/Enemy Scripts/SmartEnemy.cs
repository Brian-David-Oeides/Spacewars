using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private float _dodgeDistance = 2f;
    private bool _dodgeLeft = true; // Starts by dodging left
    private bool _dodging = false;

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
    public float speedMultiplier = 1.5f;

    // Updated method for movement to move up instead of down
    private void CalculateMovement()
    {
        if (!_dodging)
        {
            transform.Translate(Vector3.up * _speed * speedMultiplier * Time.deltaTime, Space.World);
        }
        // Destroy the enemy when it reaches a specific point (if needed)
        if (transform.position.y >= 9f)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        _playerTransform = _player.transform;

        // set initial fire time to avoid immediate firing before spawn
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

        CalculateMovement();
        // If the player's laser is fired, dodge
        DetectPlayerFire();

        if (Time.time > _canFire)
        {
            FireLasers();
        }
    }

    void DetectPlayerFire()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Detects player's laser fire
        {
            StartCoroutine(Dodge());
        }
    }

    IEnumerator Dodge()
    {
        _dodging = true;
        Vector3 dodgeDirection;

        // Toggle dodge direction
        if (_dodgeLeft)
        {
            dodgeDirection = Vector3.left;
        }
        else
        {
            dodgeDirection = Vector3.right;
        }

        // Perform dodge movement
        Vector3 dodgeTarget = transform.position + (dodgeDirection * _dodgeDistance);
        float dodgeSpeed = 5f; // Speed of dodge

        while (Vector3.Distance(transform.position, dodgeTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dodgeTarget, dodgeSpeed * Time.deltaTime);
            yield return null;
        }

        // Toggle direction for the next dodge
        _dodgeLeft = !_dodgeLeft;
        _dodging = false;
    }

    // method to adjust enemy stats based on the wave number
    public void InitializeForWave(int wave)
    {
        // increase enemy speed based on wave number (e.g., 10% increase per wave)
        _increaseWaveSpeed = _speed * (1 + (wave * 0.1f));
    }

    public void FireLasers()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        // Null check to ensure the prefab is assigned
        if (_enemyLaserPrefab == null)
        {
            Debug.LogError("_enemyLaserPrefab is not assigned!");
            return; // Exit the method to avoid further issues
        }

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
                    StartCoroutine(_cameraShake.Shake(0.3f, 0.5f));
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
