using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser = false;

    // Add direction for enemy laser movement
    private Vector3 _direction;

    private ShakeCamera _cameraShake;

    // add the Start() method to initialize the main camera
    void Start()
    {
        _cameraShake = Camera.main.GetComponent<ShakeCamera>();
    }

    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            // Choose between standard MoveDown or directional movement
            if (_direction == Vector3.zero)
            {
                MoveDown();  // Default behavior for enemy lasers without specific direction
            }
            else
            {
                MoveInDirection();  // Move in the assigned direction
            }
        }
    }

    void MoveUp()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);

        if (this.transform.position.y > 8.0f)
        {   
            if (this.transform.parent != null)
            {   
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (this.transform.position.y < -8.0f)
        {   
            if (this.transform.parent != null)
            {   
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    // New method to move the laser in the assigned direction
    void MoveInDirection()
    {
        transform.Translate(_speed * Time.deltaTime * _direction, Space.World);

        // Adjust bounds to prevent laser from going off-screen
        if (this.transform.position.y < -8.0f || this.transform.position.y > 8.0f ||
            this.transform.position.x < -9.0f || this.transform.position.x > 9.0f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    // New method to set the laser's movement direction
    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;  // Ensure the direction is normalized
    }

    public bool IsEnemyLaser
    {
        get { return _isEnemyLaser; }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                if (_cameraShake != null)
                {
                    _cameraShake.TriggerShake(0.1f, 0.2f);
                }
            }

            // Destroy the laser after hitting the player
            Destroy(this.gameObject);
        }
    }
}
