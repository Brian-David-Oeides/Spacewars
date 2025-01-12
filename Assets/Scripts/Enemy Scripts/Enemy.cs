
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour, IFireLaser
{
    [SerializeField]
    private float _speed = 2f;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private EnemyShield _shield;

    private Aggression _aggression; // reference to Aggression behavior

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake; 
    private bool _isDestroyed = false;

    public float _increaseWaveSpeed; // speed that is adjusted based on wave number
    private WaveManager _waveManager;
    private EnemyType _enemyType;

    public void Initialize(float speed, GameObject laserPrefab)
    {
        _speed = speed;
        _enemyLaserPrefab = laserPrefab;
    }

    public void SetEnemyType(EnemyType enemyType)
    {
        _enemyType = enemyType;
    }

    public void SetWaveManagerReference(WaveManager waveManager)
    {
        _waveManager = waveManager;
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
        _canFire = Time.time + Random.Range(0.2f, 3f); // random delay

        // get/initialize Aggression behavior
        _aggression = GetComponent<Aggression>();
        // check if aggression behavior exists then
        if ( _aggression != null )
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

        _increaseWaveSpeed = _speed; // set speed to increased speed based on wave

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
       
        if (Time.time > _canFire)
        {   
            FireLasers();
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
            ReturnToPool();
        }
    }


    // method to adjust enemy stats based on the wave number
    public void InitializeForWave(int wave)
    {
        // increase enemy speed based on wave number (e.g., 10% increase per wave)
        _increaseWaveSpeed = _speed * (1 + (wave * 0.1f));
        // further adjustments such as health, attack rate, etc., can be added here
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

    // match Action delegate signature to subscribe
    private void Ramming()
    {
        // Verify subscription
        Debug.Log($"{gameObject.name} is now ramming!");
    }

    private void ReturnToPool()
    {
        _isDestroyed = true;
        EnemyPoolManager.Instance.ReturnEnemy(_enemyType, this.gameObject);
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
                shieldComponent.Deactivate(); 
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
    private void OnDestroy()
    {
        if (_waveManager != null)
        {
            _waveManager.EnemyDestroyed();  // Report destruction
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
