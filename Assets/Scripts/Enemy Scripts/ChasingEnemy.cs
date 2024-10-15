using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    [SerializeField]
    protected GameObject _enemyLaserPrefab;

    private Player _player;
    private Transform _playerTransform;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;
    private ShakeCamera _cameraShake;

    protected bool _isDestroyed = false;
    private bool _isFallingBack = false;  // new flag to control state
    private float _angle;

    private float _angularSpeed;



    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();

        // set initial fire time to avoid immediate firing before spawn
        _canFire = Time.time + Random.Range(0.2f, 3f); // random delay

        if (_player != null)
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }

        _explosionAnimation = GetComponent<Animator>();

        if (_explosionAnimation == null)
        {
            Debug.LogError("The player is NULL.");
        }

        _angularSpeed = _speed * 2f;
    }

    protected virtual void Update()
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

    private void CalculateMovement()
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
                this.transform.Translate(moveDirection * _angularSpeed * Time.deltaTime, Space.World);
            }
        }

        if (_isFallingBack)  // enter fallback mode
        {
            // move down the y-axis
            this.transform.Translate(Vector3.down * _angularSpeed * Time.deltaTime);

            // if position goes below -4.6f, destroy or reset
            if (this.transform.position.y < -4.6f)
            {
                // rotate towards the negative y-axis
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // face downwards

                Destroy(this.gameObject);  // destroy chasingenemy
                Debug.Log("Chasing enemy self-destructed.");
            }
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
