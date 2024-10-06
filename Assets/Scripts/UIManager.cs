using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private Image _livesImg;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private Text _waveText; // new field for displaying wave number

    private GameManager _gameManager;

    private Coroutine _flashAmmoRoutine;

    [SerializeField]
    private Slider _thrusterSlider; 

    [SerializeField]
    private Text _thrusterPercentageText;  

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        // turn off the game object Game_Over_Text on Start
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }

        if (_thrusterSlider != null) // check that the Slider is not inactive
        {
            _thrusterSlider.maxValue = 1; // start the slider at maximum value
            _thrusterSlider.value = 1; // get the current value
        }
        else
        {
            Debug.LogError("Thruster Slider is not assigned in the Inspector.");
        }
        
        if (_thrusterPercentageText != null) // check that the Text is not inactive
        {
            UpdateThrusterPercentageText(1);  // start Text at 100% thruster charge
        }
        else
        {
            Debug.LogError("Thruster Percentage Text is not assigned in the Inspector.");
        }

        _waveText.gameObject.SetActive(false); // Hide wave text at start
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {    
        _livesImg.sprite = _liveSprites[currentLives]; 

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateShieldColor(float colorValue)
    { 
        Color newColor = Color.Lerp(Color.red, Color.white, colorValue);

        _livesImg.color = newColor;
    }
    public void ResetLivesColor()
    {
        _livesImg.color = Color.white;
    }

    public void UpdateAmmoUI(int currentAmmo)
    {
        _ammoText.text = "Ammo Count: " + currentAmmo.ToString();
    }

    public void FlashAmmoUI()
    {
        if (_flashAmmoRoutine == null)
        { 
            _flashAmmoRoutine = StartCoroutine(FlashAmmoRoutine());
        }
    }

    public void StopFlashingAmmoUI()
    {
        if (_flashAmmoRoutine != null)
        {
            StopCoroutine(_flashAmmoRoutine);
            _flashAmmoRoutine = null;
            _ammoText.color = Color.white; 
        }
    }

    IEnumerator FlashAmmoRoutine()
    {
        while (true)
        {
            _ammoText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            _ammoText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartThrusterSlider(float thrustDuration, float thrustCooldown)
    {
        if (_thrusterSlider != null) 
        {
            _thrusterSlider.value = 1; 
            UpdateThrusterPercentageText(1); 
            StartCoroutine(ThrusterSliderRoutine(thrustDuration, thrustCooldown));
        }
    }

    private IEnumerator ThrusterSliderRoutine(float thrustDuration, float thrustCooldown)
    {
        float elapsedTime = 0; 

        while (elapsedTime < thrustDuration)
        {
            elapsedTime += Time.deltaTime;  
            _thrusterSlider.value = Mathf.Lerp(1, 0, elapsedTime / thrustDuration);
            UpdateThrusterPercentageText(_thrusterSlider.value);  
            yield return null;
        }

        // start cooldown for thrusters
        elapsedTime = 0;
        while (elapsedTime < thrustCooldown)
        {
            elapsedTime += Time.deltaTime;
            _thrusterSlider.value = Mathf.Lerp(0, 1, elapsedTime / thrustCooldown);
            UpdateThrusterPercentageText(_thrusterSlider.value);
            yield return null;
        }

        ResetThrusterSlider();
    }

    public void ResetThrusterSlider()
    {
        if (_thrusterSlider != null)
        {
            _thrusterSlider.value = 1; 
            UpdateThrusterPercentageText(1);  
        }
    }

    private void UpdateThrusterPercentageText(float sliderValue)
    {
        if (_thrusterPercentageText != null)
        {
            int percentage = Mathf.RoundToInt(sliderValue * 100);
            _thrusterPercentageText.text = percentage.ToString() + "%";
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
    // method to display current wave number
    public void DisplayWave(int waveNumber)
    { 
        _waveText.text = $"Wave {waveNumber} Starting!"; // set wave text
        _waveText.gameObject.SetActive(true);  // show  wave text
        StartCoroutine(HideWaveTextAfterDelay()); // hide after delay
    }

    IEnumerator HideWaveTextAfterDelay()
    {
        yield return new WaitForSeconds(2f); // display wave text for 2 seconds
        _waveText.gameObject.SetActive(false); // hide wave text
    }
}
