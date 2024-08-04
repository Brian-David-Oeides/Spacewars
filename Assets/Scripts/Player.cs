using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2;
    // private float _speed2 = 7.0f;
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
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _Shield;

    [SerializeField]
    private int _score; 
    private UIManager _uiManager;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
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
        transform.Translate(_speed * Time.deltaTime * direction);
        

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

        // when variable triple shot powerup collected = on
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else // or else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            // disable reference data type to shield

            _Shield.SetActive(false);
            return; 
        }

        _lives -=1;

        
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        // tripleShotActive becomes true
        _isTripleShotActive = true;
        // start the power down coroutine for triple shot
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    { 
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplier;
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        // enable the reference data type for shield sprite
        _Shield.SetActive(true);
    }

    // create method add 10 to this game object’s score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    // communicate with UI to update the score
}
