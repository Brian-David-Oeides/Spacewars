using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class SideToSideEnemy : MonoBehaviour, IFireLaser
{
    [SerializeField]
    private float _speed = 2f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private EnemyShield _shield; 

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake;
    private bool _isDestroyed = false;

    private float _amplitude = 4.0f;
    private float _frequency = 6.0f;
    private float _timeCounter = 0.0f;

    private EnemyType _enemyType;

    public void Initialize(float speed, GameObject laserPrefab, EnemyType enemyType)
    {
        _enemyType = enemyType;
        _speed = speed;
        _enemyLaserPrefab = laserPrefab;

        // Reset enemy for reuse in the pool
        _isDestroyed = false;
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = true;
    }

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

        _shield = GetComponentInChildren<EnemyShield>();

        
    }

    private void CalculateMovement()
    {
        _timeCounter += Time.deltaTime * _frequency;
        float x = Mathf.Cos(_timeCounter) * _amplitude;
        float downwardSpeed = _speed; // slow down _speed = 1
        float newY = this.transform.position.y - downwardSpeed * Time.deltaTime;
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
                player.Damage(1);
                if (_cameraShake != null)
                {
                    _cameraShake.TriggerShake(0.1f, 0.2f);
                }
            }

            TriggerEnemyDeath();
            ReturnToPool();
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
            ReturnToPool();
        }

    }
    private void ReturnToPool()
    {
        if (_enemyType != null)
        {
            gameObject.SetActive(false);
            EnemyPoolManager.Instance.ReturnEnemy(_enemyType, this.gameObject);
        }
        else
        {
            Debug.LogWarning($"No EnemyType assigned for {gameObject.name}. Destroying instead.");
            Destroy(gameObject, 2.8f);
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

        WaveManager waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();

        if (waveManager != null)
        {
            waveManager.EnemyDestroyed(); // notify WaveManager about enemy's destruction
        }
        else
        {
            Debug.LogError("WaveManager is NULL or not found!");
        }
    }
}

