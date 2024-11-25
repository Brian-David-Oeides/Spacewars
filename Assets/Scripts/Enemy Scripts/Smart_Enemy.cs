using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smart_Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2;
   
    [SerializeField]
    private GameObject _smartLaserPrefab; 
    private Player _player;

    private float _fireRate = 1.25f;
    private float _canFire = -1f;
    private bool _isDestroyed = false;
    private ShakeCamera _cameraShake;
    private Animator _explosionAnimation;
    private AudioSource _audioSource;

    [SerializeField]
    private float _detectionRange = 5f; 
    private bool _isDodging = false; 

    [SerializeField]
    private float _dodgeDistance = 2f; 
    private bool _dodgingRight = true; 


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

        if (Vector3.Distance(_player.transform.position, transform.position) <= 6f)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_isDestroyed) return;

        MoveUpward();
        
        if (_player != null)
        {
            DetectAndDodgePlayerLaser();

            if (transform.position.y > _player.transform.position.y + 3.5f)
            {
                FireLasers();
            }
        }

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

            Vector3 directionToPlayer = _player.transform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f;

            GameObject laser = Instantiate(_smartLaserPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Laser[] lasers = laser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser(); 
            }
        }
    }

    private void DetectAndDodgePlayerLaser() 
    { 
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Laser") && !_isDodging) 
            {
                StartCoroutine(Dodge());
                break; 
            }
        }
    }

    private IEnumerator Dodge()
    {
        _isDodging = true;
 
        float dodgeDirection = _dodgingRight ? _dodgeDistance : -_dodgeDistance;
        _dodgingRight = !_dodgingRight; 

        Vector3 dodgePosition = new Vector3(transform.position.x + dodgeDirection, transform.position.y, 0);
        dodgePosition.x = Mathf.Clamp(dodgePosition.x, -9f, 9f); 

        float dodgeTime = 0.10f; 
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        while (elapsedTime < dodgeTime)
        {
            transform.position = Vector3.Lerp(startPosition, dodgePosition, elapsedTime / dodgeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = dodgePosition; 

        yield return new WaitForSeconds(1f); 

        _isDodging = false;
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

            if (_player != null)
            {
                _player.AddScore(10);
            }

            TriggerEnemyDeath();
        }
    }

    private void TriggerEnemyDeath()
    {
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

        Destroy(this.gameObject, 1.8f);
    }
}
