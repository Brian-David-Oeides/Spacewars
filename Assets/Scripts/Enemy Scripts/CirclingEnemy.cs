using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingEnemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 4f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private EnemyShield _shield; // reference EnemyShield class

    private Aggression _aggression; // reference to Aggression behavior

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

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        _canFire = Time.time + Random.Range(0.2f, 3f); 

        // get/initialize Aggression behavior
        _aggression = GetComponent<Aggression>();
        // check if aggression behavior exists then
        if (_aggression != null)
        {
            // subscribe to the delegate Ramming
            _aggression.Ramming += Ramming;
            // set the ramming range
            _aggression.SetRammingRange(3f);
        }
        else // or else log a warning the aggression component is missing
        {
            Debug.LogWarning($"Aggression component is missing on {gameObject.name}");
        }

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
            _isCircling = true;

            _circleCenter = new Vector3(transform.position.x - _circleRadius, transform.position.y, 0);
        }

        if (_isCircling)
        {
            _circleAngle -= _speed * Time.deltaTime;
            float xOffset = Mathf.Cos(_circleAngle) * _circleRadius;
            float yOffset = Mathf.Sin(_circleAngle) * _circleRadius;

            Vector3 currentPosition = new Vector3(_circleCenter.x + xOffset, _circleCenter.y + yOffset, 0);
            transform.position = currentPosition;

            Vector3 direction = currentPosition - _circleCenter;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (_circleAngle <= -2 * Mathf.PI)
            {
                _circleAngle = 0f;   
                _isCircling = false;
                _circleCompleted = true; 
            }
        }
        else if (!_circleCompleted)
        {
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);
        }


        if (_circleCompleted)
        {
            transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);

            if (transform.position.y < -5f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void FireLasers()
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

    private void Ramming()
    {
        // Verify subscription
        Debug.Log($"{gameObject.name} is now ramming!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(1);
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

            // Try to get the shield component dynamically
            EnemyShield shieldComponent = GetComponentInChildren<EnemyShield>();

            if (shieldComponent != null && shieldComponent.AbsorbHit())
            {
                shieldComponent.Deactivate(); // Turn off shield animation
                Debug.Log("Hit was absorbed; enemy remains!");
                return;
            }

            if (_player != null)
            {
                _player.AddScore(10);
            }

            TriggerEnemyDeath();
        }

    }
    
    private void TriggerEnemyDeath()
    {
        // if aggression exists unsubscribe 
        if (_aggression != null)
        {
            // unsubscribe from the aggression event
            _aggression.Ramming -= Ramming;
        }

        _explosionAnimation.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isDestroyed = true;
  
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        WaveManager waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();

        if (waveManager != null)
        {
            waveManager.EnemyDestroyed(); 
        }
        else
        {
            Debug.LogError("WaveManager is NULL or not found!");
        }

        Destroy(this.gameObject, 2.8f);
    }
}

