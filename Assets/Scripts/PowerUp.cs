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
    
    private bool _isDestroyed = false;  // track if already destroyed

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

    }

    void Update()
    {
        if (_isDestroyed) return;  // prevent further movement during explosion

        if (_moveToPlayer && _playerTransform != null)
        {
            float step = _speedToPlayer * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, step);
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
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collision detected with tag: {other.tag}");

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
        else if (other.CompareTag ("Enemy_Laser"))
        {
            Debug.Log("Enemy laser collided with PowerUp, destroying laser.");
            Destroy(other.gameObject);  // destroy enemy laser
            Destroy(this.gameObject); // destroy power-up
            
        }
    }
}
