
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 2f;
    protected float _fireRate = 3.0f;
    protected float _canFire = -1;

    protected bool _canBaseFire = true; // default for base class

    [SerializeField]
    protected GameObject _enemyLaserPrefab;

    protected Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake; 
    protected bool _isDestroyed = false;

    protected float _increaseWaveSpeed;     // speed that is adjusted based on wave number

    // add virtual method for calculate movement 
    protected virtual void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    protected virtual void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();

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
            Debug.LogError("The player is NULL.");
        }
    }

    protected virtual void Update()
    {
        if (_isDestroyed)
        {
            return;
        }

        CalculateMovement(); 
        // _canFire enabled
        if (_canBaseFire && Time.time > _canFire)
        {   
            FireLasers();
        }
    }

    // method to adjust enemy stats based on the wave number
    public void InitializeForWave(int wave)
    {
        // increase enemy speed based on wave number (e.g., 10% increase per wave)
        _increaseWaveSpeed = _speed * (1 + (wave * 0.1f));
        // further adjustments such as health, attack rate, etc., can be added here
    }

    public void SetLaserPrefab(GameObject laserPrefab)
    {
        _enemyLaserPrefab = laserPrefab;
    }

    // create new FireLasers() method
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
