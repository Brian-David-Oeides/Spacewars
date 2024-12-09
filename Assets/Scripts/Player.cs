using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private float _thrustDuration = 5.0f;
    [SerializeField]
    private float _thrustCooldown = 10.0f;

    private float _thrusterSpeed;

    private SmartMissile smartMissile;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _multiShotPrefab;
    [SerializeField]
    private GameObject _smartMissilePrefab;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    private Thruster _thruster;

    private bool _isSmartMissileActive = false; // flag smart missile
    private bool _isTripleShotActive = false;
    private bool _isMultiShotActive = false;
    private bool _isShieldActive = false;
    private bool _canFireLaser = true; // disable the firelaser

    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private int _shieldsRemaining;

    [SerializeField]
    private GameObject _damagedLeftEngine, _damagedRightEngine;

    [SerializeField]
    private int _score;
    
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _outOfAmmo;

    [SerializeField]
    private int _maxAmmo = 15;
    private int _currentAmmo;

    void Start()
    {
        _thruster = new Thruster(_speed, _speedMultiplier, _thrustDuration, _thrustCooldown);
        
        // Subscribe to Thruster events
        _thruster.OnThrusterActivated += HandleThrusterActivated;
        _thruster.OnThrusterDeactivated += HandleThrusterDeactivated;

        transform.position = new Vector3(0, -1.68f, 0);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        _currentAmmo = _maxAmmo;

        //_thrusterSpeed = _speed;

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
        CalculateMovement(_thruster.CurrentSpeed);
        //HandleThrusterInput();

        // check to see if FireLaser is turned on
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _canFireLaser)
        {
            FireLaser();
        }

        // Pass input to Thruster
        bool isThrusterKeyPressed = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift);
        _thruster.HandleInput(isThrusterKeyPressed);

    }

    public float GetPlayerDirectionX()
    {
        return Input.GetAxis("Horizontal");
    }

    void CalculateMovement(float speed)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(speed * Time.deltaTime * direction);

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

    private void HandleThrusterActivated(float newSpeed)
    {
        Debug.Log($"Thrusters Activated! Speed: {newSpeed}");
        _uiManager.StartThrusterSlider(_thrustDuration, _thrustCooldown);
    }

    private void HandleThrusterDeactivated()
    {
        Debug.Log("Thrusters Deactivated!");
        _uiManager.ResetThrusterSlider();
    }
    
    
    void FireLaser()
    {
        if (_currentAmmo > 0)
        {
            _canFire = Time.time + _fireRate;

            if (_isSmartMissileActive == true)
            {
                FireSmartMissile();
            }
            else if (_isMultiShotActive == true)
            {
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

            _currentAmmo--;
            _uiManager.UpdateAmmoUI(_currentAmmo);

            if (_currentAmmo <= 0)
            {
                _canFireLaser = false; 
                _audioSource.PlayOneShot(_outOfAmmo);
                _uiManager.FlashAmmoUI();
            }
        }
    }

    public void DisableFireLaser() 
    {
        
        StartCoroutine(DisableFireLaserRoutine()); 
        _audioSource.PlayOneShot(_outOfAmmo); 
    }

    private IEnumerator DisableFireLaserRoutine()
    {
        _canFireLaser = false;  
        yield return new WaitForSeconds(5.0f);  
        _canFireLaser = true;  
    }


    public void Damage(int Damage)
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
    public void FireSmartMissile()
    {
        Instantiate(_smartMissilePrefab, transform.position + new Vector3(-1.0f, 0, 0), Quaternion.identity);
        Instantiate(_smartMissilePrefab, transform.position + new Vector3(1.0f, 0, 0), Quaternion.identity);
    }
    public void SmartMissileActive()// custom method public void SmartMissile()
    {
        _isSmartMissileActive = true; // enable _isSmartMissileActive
        StartCoroutine(SmartMissilePowerDownRoutine());// StartCoroutine power down SmartMissile
    }

    IEnumerator SmartMissilePowerDownRoutine() // coroutine for power down routine
    {
        yield return new WaitForSeconds(5.0f); // yield a 5 second delay 
        _isSmartMissileActive = false; // disable _isSmartMissileActive 
    }
    public void MultiShotActive() 
    {
        _isMultiShotActive = true; 
        StartCoroutine(MultiShotPowerDownRoutine());
    }

    IEnumerator MultiShotPowerDownRoutine() 
    {
        yield return new WaitForSeconds(5.0f); 
        _isMultiShotActive = false; 
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
        _thruster.UpdateBaseSpeed(_speed * _speedMultiplier);
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _thruster.UpdateBaseSpeed(_speed);

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

    public void HealthPowerUp() 
    {
        if (_lives < 3) 
        {
            _lives++; 
            _uiManager.UpdateLives(_lives); 
        }

        ResetDamageIndicators();
    }

    private void ResetDamageIndicators() 
    {
        if (_damagedRightEngine.activeSelf)
        {
            _damagedRightEngine.SetActive(false); 
        }
        else if (_damagedLeftEngine.activeSelf)
        {
            _damagedLeftEngine.SetActive(false); 
        }
         
        _uiManager.ResetLivesColor(); 

    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    
    public int GetLives() // getter for _lives
    {
        return _lives;
    }

    public int GetCurrentAmmo() // getter for _currentAmmo
    {
        return _currentAmmo;
    }

    public int GetMaxAmmo() // Getter for _maxAmmo
    {
        return _maxAmmo;
    }
}
