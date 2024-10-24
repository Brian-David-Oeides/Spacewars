﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smart_Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2;
   
    [SerializeField]
    private GameObject _smartLaserPrefab; // Reference to the SmartEnemy's laser prefab
    private Player _player;
    private float _spawnY = -6.85f;
    private float _minX = -9f;
    private float _maxX = 9f;
    private float _fireRate = 1.25f;
    private float _canFire = -1f;
    private bool _isDestroyed = false;
    private ShakeCamera _cameraShake;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    [SerializeField]
    private float _dodgeDistance = 2f; // How far the enemy moves when dodging

    [SerializeField]
    private float _detectionRange = 5f; // Detection range for player's laser
    private bool _dodgingRight = true; // Tracks which direction the enemy last dodged
    private bool _isDodging = false; // Tracks if the enemy is currently dodging

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        _explosionAnimation = GetComponent<Animator>();
        if (_explosionAnimation == null)
        {
            Debug.LogError("The player is NULL.");
        }

        transform.position = new Vector3(Random.Range(_minX, _maxX), _spawnY, 0);
        transform.rotation = Quaternion.Euler(0, 0, 180);

        // Check if player is at a safe distance before spawning
        if (Vector3.Distance(_player.transform.position, transform.position) <= 6f)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_isDestroyed) return;

        MoveUpward();
        // If player exists, continue detecting and dodging lasers and firing
        if (_player != null)
        {
            DetectAndDodgePlayerLaser();
            // Check if the SmartEnemy has passed the player by 1.5f on the y-axis
            if (transform.position.y > _player.transform.position.y + 2.5f)
            {
                FireLasers();
            }
        }

        // Destroy the SmartEnemy if it reaches the top of the screen
        if (transform.position.y >= 9f)
        {
            Destroy(gameObject);
        }
    }

    private void MoveUpward()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.World);
    }

    private void FireLasers()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;

            // Calculate direction to player
            Vector3 directionToPlayer = _player.transform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;

            GameObject laser = Instantiate(_smartLaserPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Laser[] lasers = laser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser(); // Assuming this method assigns the laser to the SmartEnemy
            }
        }
    }

    private void DetectAndDodgePlayerLaser() 
    {
        // Detect all lasers within range using a physics overlap method
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Laser") && !_isDodging) // Assuming Player's lasers are tagged as "Laser"
            {
                StartCoroutine(Dodge());
                break; // Dodge on detecting one laser
            }
        }
    }

    private IEnumerator Dodge()
    {
        _isDodging = true;

        // Calculate dodge direction based on last dodge direction
        float dodgeDirection = _dodgingRight ? _dodgeDistance : -_dodgeDistance;
        _dodgingRight = !_dodgingRight; // Alternate dodge direction

        // Perform the dodge
        Vector3 dodgePosition = new Vector3(transform.position.x + dodgeDirection, transform.position.y, transform.position.z);
        dodgePosition.x = Mathf.Clamp(dodgePosition.x, _minX, _maxX); // Ensure the enemy doesn't go out of bounds

        float dodgeTime = 0.25f; // Time it takes to dodge
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        while (elapsedTime < dodgeTime)
        {
            transform.position = Vector3.Lerp(startPosition, dodgePosition, elapsedTime / dodgeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = dodgePosition; // Ensure it reaches the exact dodge position

        yield return new WaitForSeconds(1f); // Cooldown before detecting lasers again

        _isDodging = false;
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

        Destroy(this.gameObject, 1.8f);
    }
}
