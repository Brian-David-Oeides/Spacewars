using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    [SerializeField]
    private GameObject _homingMissilePrefab;

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake;
    private bool _isDestroyed = false;

    public float _increaseWaveSpeed;     // speed that is adjusted based on wave number

    private void CalculateMovement()
    {

        // move horizontally right
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x > 13.80f)
        {
            float randomY = Random.Range(5f, 7f);
            transform.position = new Vector3(-14.85f, randomY, 0);
        }
    }

    void Start()
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

    void Update()
    {
        if (_isDestroyed)
        {
            return;
        }

        CalculateMovement();

        if (transform.position.x >= -10f)
        {
            if (Time.time > _canFire)
            {
                FireLasers();
            }
        }
    }
    private void FireLasers()
    {
        _fireRate = Random.Range(3f, 4f);
        _canFire = Time.time + _fireRate;

        // instantiate homing missile laser
        GameObject homingMissile = Instantiate(_homingMissilePrefab, this.transform.position, Quaternion.identity);
        homingMissile.GetComponent<HomingMissile>(); // use HomingMissile script
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

        // notify the SpawnManager that the Horizontal Enemy has been destroyed
        SpawnManager spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if (waveManager != null)
        {
            waveManager.EnemyDestroyed(); // notify WaveManager about enemy's destruction
        }
        else
        {
            Debug.LogError("WaveManager is NULL or not found!");
        }

        if (spawnManager != null) // check if SpawnManager was found
        {
            // notify SpawnManager Horizontal Enemy  destroyed
            spawnManager.OnHorizontalEnemyDestroyed();
        }
        else
        {
            Debug.LogError("SpawnManager NULL! Unable to notify of HorizontalEnemy destruction.");
        }

        Destroy(this.gameObject, 2.8f);
    }

}
