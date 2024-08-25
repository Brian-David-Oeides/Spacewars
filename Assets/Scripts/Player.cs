using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private float _thrust = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab; 
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private int _shieldsRemaining;

    [SerializeField] 
    private GameObject _damagedLeftEngine, _damagedRightEngine;

    [SerializeField]
    private int _score; 
    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource; 


    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void Update()
    {
        CalculateMovement();
     
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
           FireLaser();
        }
       
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput,0);
        

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.Translate((_speed *_thrust) * Time.deltaTime * direction);
        }
        else 
        {
            transform.Translate(_speed * Time.deltaTime * direction);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.29f)
        {
            transform.position = new Vector3(-11.29f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.29f)
        {
            transform.position = new Vector3(11.29f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void Damage()
    {   // if shield is active
        if (_isShieldActive == true)
        {
            _shieldsRemaining--;

            if (_shieldsRemaining <= 0)
            {
                _isShieldActive = false; //deactivate shield
                _shield.SetActive(false); // deactivate visual representation
                // Reset the color of the lives image after shield deactivates
                _uiManager.ResetLivesColor();
            }
            else
            {
                // Change shield color based on remaining collisions
                float colorValue = _shieldsRemaining / 3f; 
                // get the color of the shield sprite set it to a new color
                _shield.GetComponent<SpriteRenderer>().color = new Color(1f, colorValue, colorValue, 1f);
                // Communicate with UIManager to update shield color
                _uiManager.UpdateShieldColor(colorValue);
            }
            return; 
        }

        _lives -=1;
        
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _damagedRightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _damagedLeftEngine.SetActive(true);
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;

        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    { 
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplier;
        
    }

    public void ShieldActive()
    {
        _isShieldActive = true; // shields activate on power up collected
        _shieldsRemaining = 3; // access the 3 shields
        _shield.SetActive(true); // representaion of shield is set to true

        // Get the shield's color and set to white 
        _shield.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
