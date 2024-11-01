using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    [SerializeField] // 0 = Triple Shot, 1 = Speed, 2 = Shield, 3 = Ammo. 4 = Health, 5 = MultiShot, 6 = DisableFireLaser, 7 = SmartMissile
    private int _powerUpID;
    [SerializeField]
    private AudioClip _clip;
    
    private AudioSource _explosionAudio;  // audio for explosion

    private Animator _explosionAnimation;
    private bool _isExploding = false;  // Track if already exploding

    private Transform _playerTransform; // reference to player
    [SerializeField]
    private float _speedToPlayer = 10.0f; // speed when moving towards player
    [SerializeField]
    private float _triggerRange = 4.0f; // range power-up can move to player
    private bool _moveToPlayer = false; // enable and disable moving to Player
    
    private void Start()
    {
        // Find the player object in the scene
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }

        _explosionAnimation = GetComponent<Animator>();
        _explosionAudio = GetComponent<AudioSource>();

        if (_explosionAnimation == null)
        {
            Debug.LogError("Animator component missing on PowerUp.");
        }
    }

    void Update()
    {
        if (_isExploding) return;  // Prevent further movement during explosion

        if (_moveToPlayer && _playerTransform != null)
        {
            float step = _speedToPlayer * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, step);

            Debug.Log("Speed to player: " + _speedToPlayer);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);
        }

        if (this.transform.position.y < -4.7f)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.C) && _playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _triggerRange)
            {
                _moveToPlayer = true;
            }

            Debug.Log("Distance to player: " + distanceToPlayer);
            Debug.Log("Moving to player: " + _moveToPlayer);
        }
    }

    void OnDrawGizmos()
    {
        if (_playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_playerTransform.position, _triggerRange);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position); 

            switch(_powerUpID)
            {
                case 0:
                    player.TripleShotActive();
                    break;
                case 1:
                    player.SpeedBoostActive(); 
                    break;
                case 2:
                    player.ShieldActive();
                    break;
                case 3:
                    player.AmmoActive();
                    break;              
                case 4:
                    player.HealthPowerUp();
                    break;
                case 5: 
                    player.MultiShotActive(); 
                    break;
                case 6: 
                    player.DisableFireLaser(); 
                    break;
                case 7: // add smart missile
                    player.SmartMissileActive(); 
                    break;
                default:
                    Debug.Log("Default Value");
                    break;
            }

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Enemy_Laser"))
        {
            Destroy(other.gameObject);  // Destroy the enemy laser
            TriggerExplosion();
        }
    }
    private void TriggerExplosion()
    {
        _isExploding = true;  // Set exploding state

        // Disable collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        _explosionAnimation.SetTrigger("Explosion");  // Trigger explosion animation
        _explosionAnimation.SetLayerWeight(1, 1);     // Activate ExplosionLayer by setting its weight to 1
        _explosionAudio.Play();  // Play explosion sound
        Destroy(this.gameObject, 1.0f);  // Delay destruction to allow explosion animation to play
    }
}
