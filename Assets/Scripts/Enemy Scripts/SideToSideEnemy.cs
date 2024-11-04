using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSideEnemy : MonoBehaviour, IFireLaser
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

    // lateral movement boundaries
    private float _amplitude = 4.0f;
    // lateral movement speed (affects how fast the enemy moves side to side)
    private float _frequency = 6.0f;
    // time tracker for the cosine function
    private float _timeCounter = 0.0f;

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
    }

    private void CalculateMovement()
    {
        // increment based on real-time * frequency
        _timeCounter += Time.deltaTime * _frequency;
        // calculate X position based on cosine function 
        float x = Mathf.Cos(_timeCounter) * _amplitude;

        // calculate downward movement 
        float downwardSpeed = _speed; // slow down _speed = 1
        float newY = this.transform.position.y - downwardSpeed * Time.deltaTime;

        // set new position for enemy (oscillating X movement + downward Y movement)
        this.transform.position = new Vector3(x, newY, this.transform.position.z);

    }

    void Update()
    {
        if (_isDestroyed)
        {
            return;
        }

        CalculateMovement();
        // _canFire enabled
        if (Time.time > _canFire)
        {
            FireLasers();
        }
    }

    public void FireLasers()
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

