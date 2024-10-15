using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingEnemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 4f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake;
    private bool _isDestroyed = false; [SerializeField]



    private float _circleRadius = 3.5f; // radius of the circle
    private bool _isCircling = false; // Start by moving downward
    private bool _circleCompleted = false;  // Counter for completed circles
    private Vector3 _circleCenter; // Center of the circle
    private float _circleAngle = 0f; // Angle to calculate circle position
    private float _angularSpeed; // Angular speed for consistent movement

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

    protected virtual void Update()
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
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);
        }

        // once the circle is done, continue downward movement normally
        if (_circleCompleted)
        {
            // continue moving down 
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);

            // destroy the object if it goes out of bounds
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

