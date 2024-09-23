
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 2f;
    protected float _fireRate = 3.0f;
    protected float _canFire = -1;

    [SerializeField]
    protected GameObject _enemyLaserPrefab;

    protected Player _player;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    private ShakeCamera _cameraShake; 
    protected bool _isDestroyed = false;

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
        
        if (_player == null)
        {
            Debug.LogError("The player is NULL.");
        }
        
        _explosionAnimation = GetComponent<Animator>();
        
        if (_explosionAnimation == null)
        {
            Debug.LogError("The player is NULL.");
        }

        // set initial fire time to avoid immediate firing before spawn
        _canFire = Time.time + Random.Range(0.5f, 3f); // random delay 

    }

    void Update()
    {
        if (_isDestroyed)
        {
            return;
        }


        CalculateMovement(); // polymorphism used for movement

        if (Time.time > _canFire)
        {   // declare cache of FireLasers() method 
            FireLasers();
        }
    }

    public void SetLaserPrefab(GameObject laserPrefab)
    {
        _enemyLaserPrefab = laserPrefab;
    }

    // create new FireLasers() method
    protected virtual void FireLasers()
    {
        _fireRate = Random.Range(3f, 5f);
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
        Destroy(this.gameObject, 2.8f);
    }
}
