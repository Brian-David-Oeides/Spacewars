using System.Collections;
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
        if (_currentAmmo > 0)
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
