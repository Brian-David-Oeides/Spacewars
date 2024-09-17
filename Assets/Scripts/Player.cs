using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f; // normal speed
    [SerializeField]
    private float _speedMultiplier = 2.0f; // doubles the speed x2 in speed boost power up routine
    private float _thrusterSpeed;

    [SerializeField]
    private float _thrustDuration = 5.0f;  // duration thrusters are active
    [SerializeField]
    private float _thrustCooldown = 10.0f;  // cooldown duration before thrusters can be activated again
    //private bool _isThrusterActive = false;  // tracks if thrusters are active
    private bool _isThrustOnCooldown = false;  // tracks if thrusters are on cooldown


    [SerializeField]
    private GameObject _laserPrefab; 
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _multiShotPrefab;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;

    private bool _isMultiShotActive = false;

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

    private bool _canFireLaser = true;

    [SerializeField]
    private int _maxAmmo = 15;
    private int _currentAmmo;

    [SerializeField]
    private AudioClip _outOfAmmo;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        // current ammo count starts at max ammo count 
        _currentAmmo = _maxAmmo;

        _thrusterSpeed = _speed; // starting point for thruster speed is normal speed 

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
        HandleThrusterInput();

        // check to see if FireLaser is turned on
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _canFireLaser)
        {
           FireLaser();
        }

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput,0);
        
        transform.Translate(_thrusterSpeed * Time.deltaTime * direction);

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

    private void HandleThrusterInput()
    {
        // Activate thrusters if A or D and LeftShift is pressed && not in cooldown
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift) && !_isThrustOnCooldown)
        {
            ActivateThrusters(); // then activate thrusters
        }
    }

    private void ActivateThrusters()
    {
        //_isThrusterActive = true; // set thrusters to true
        _isThrustOnCooldown = true; // start the thrust cooldown timer
        _thrusterSpeed = _speed * _speedMultiplier; // thrusterspeed increase

        // update the UIManager to show thruster activation and start countdown
        _uiManager.StartThrusterSlider(_thrustDuration, _thrustCooldown);
        Invoke("DeactivateThrusters", _thrustDuration); // automatically deactivate thrusters after 5 seconds
    }

    private void DeactivateThrusters()
    {
        //_isThrusterActive = false; // disable thrusters
        _thrusterSpeed = _speed; // reset speed to normal

        // delay 10 seconds then call reset
        Invoke("ResetThrusterCooldown", _thrustCooldown);
    }

    private void ResetThrusterCooldown()
    {
        _isThrustOnCooldown = false;
        _uiManager.ResetThrusterSlider(); // notify UIManager that thrusters can be reactivated
    }

    void FireLaser()
    {
        if (_currentAmmo > 0)
        {
            _canFire = Time.time + _fireRate;

            
            if (_isMultiShotActive == true) // check if multishot power up was collected
            {   // Instantiate multishot
                Instantiate(_multiShotPrefab, transform.position, Quaternion.identity);
            }
            else if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }

            _audioSource.Play();

            // decrease current ammo in increments of 1
            _currentAmmo--;
            // Update the ammo UI
            _uiManager.UpdateAmmoUI(_currentAmmo); 

            // Check if ammo is 0 after firing
            if (_currentAmmo <= 0)
            {
                _canFireLaser = false; // Disable firing
                // play out of ammo audio one time
                _audioSource.PlayOneShot(_outOfAmmo);
                // access the UI Manager script coroutine
                _uiManager.FlashAmmoUI();
            }
        }
    }

    public void Damage()
    {   
        if (_isShieldActive == true)
        {
            _shieldsRemaining--;

            if (_shieldsRemaining <= 0)
            {
                _isShieldActive = false; 
                _shield.SetActive(false); 
                
                _uiManager.ResetLivesColor();
            }
            else
            {
                float colorValue = _shieldsRemaining / 3f; 
                _shield.GetComponent<SpriteRenderer>().color = new Color(1f, colorValue, colorValue, 1f);
                
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

    public void MultiShotActive() // multishot power up method
    {
        // enable the Multishot
        _isMultiShotActive = true;
        // start a coroutine 
        StartCoroutine(MultiShotPowerDownRoutine());
    }

    IEnumerator MultiShotPowerDownRoutine() // method to stop coroutine
    {
        yield return new WaitForSeconds(5.0f); // delay for 5 sec
        _isMultiShotActive = false; // disable power up
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
        _isShieldActive = true; 
        _shieldsRemaining = 3; 
        _shield.SetActive(true); 

        _shield.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void AmmoActive()
    {
        _currentAmmo = _maxAmmo; 
        _canFireLaser = true; 

        _uiManager.StopFlashingAmmoUI();
        _uiManager.UpdateAmmoUI(_currentAmmo);
    }

    // method to increase player health when Health Power-Up is collected
    public void HealthPowerUp() 
    {
        if (_lives < 3) // if lives is 3
        {
            _lives++; // add lives increments 1
            _uiManager.UpdateLives(_lives); // update the UI to reflect the new lives count
        }

        // reset any animations or settings related to lives
        ResetDamageIndicators();
    }

    private void ResetDamageIndicators() // method resets damage animations and lives color
    {
        if (_damagedRightEngine.activeSelf)
        {
            _damagedRightEngine.SetActive(false); // set damage left enging to false
        }
        else if (_damagedLeftEngine.activeSelf)
        {
            _damagedRightEngine.SetActive(false); // set damage right engine false
        }
         
        _uiManager.ResetLivesColor(); // reset color changes to shield damage

    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
