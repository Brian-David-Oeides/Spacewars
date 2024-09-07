using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    // declare reference data type
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

    private GameManager _gameManager;

    private Coroutine _flashAmmoRoutine;

    [SerializeField]
    private Slider _thrusterSlider; // reference slot for Slider object - thrusters

    // New variable for percentage text
    [SerializeField]
    private Text _thrusterPercentageText;  // reference Text component of thruster percentage

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
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        
        _livesImg.sprite = _liveSprites[currentLives]; // Update Sprites to reflect the number of lives

        if (currentLives == 0)
        {
            GameOverSequence();
        }

    }

    public void UpdateShieldColor(float colorValue)
    {
        // define and set the Color and change color 
        Color newColor = Color.Lerp(Color.red, Color.white, colorValue);
        // access the Color property of the lives image sprite
        _livesImg.color = newColor;
    }
    public void ResetLivesColor()
    {
        // Reset the color of the lives image to the default color (white)
        _livesImg.color = Color.white;
    }

    public void UpdateAmmoUI(int currentAmmo)
    {
        // update the ammo display on the UI
        _ammoText.text = "Ammo Count: " + currentAmmo.ToString();
    }

    public void FlashAmmoUI()
    {
        if (_flashAmmoRoutine == null)
        { 
            // Start a new flashing routine for the ammo UI
            _flashAmmoRoutine = StartCoroutine(FlashAmmoRoutine());
        }
    }

    public void StopFlashingAmmoUI()
    {
        // check if the flashAmmoRoutin is on ...
        if (_flashAmmoRoutine != null)
        {
            // then flashing text
            StopCoroutine(_flashAmmoRoutine);
            // disable the component
            _flashAmmoRoutine = null;
            // reset the color to default
            _ammoText.color = Color.white; 
        }
    }

    IEnumerator FlashAmmoRoutine()
    {
        while (true)
        {
            // set text color to red
            _ammoText.color = Color.red;
            // wait .5 sec
            yield return new WaitForSeconds(0.5f);
            // set text color to red
            _ammoText.color = Color.white;
            // wait .5 sec
            yield return new WaitForSeconds(0.5f);
        }
    }

    // start thruster slider with thrust duration and cooldown
    public void StartThrusterSlider(float thrustDuration, float thrustCooldown)
    {
        if (_thrusterSlider != null) // check if thrusterslider is not inactive
        {
            _thrusterSlider.value = 1; // set slider to full to show thrusters are active
            UpdateThrusterPercentageText(1);  // update percentage to 100%
            StartCoroutine(ThrusterSliderRoutine(thrustDuration, thrustCooldown));// start Coroutine
        }
    }

    // coroutine to manage the slider value for both thrust duration and cooldown
    private IEnumerator ThrusterSliderRoutine(float thrustDuration, float thrustCooldown)
    {
        float elapsedTime = 0; // initialize time elapsed

        // count down the thruster duration
        while (elapsedTime < thrustDuration)
        {
            elapsedTime += Time.deltaTime; // add time 
            _thrusterSlider.value = Mathf.Lerp(1, 0, elapsedTime / thrustDuration);
            UpdateThrusterPercentageText(_thrusterSlider.value);  // update percentage text based on slider value
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

    // method to reset the thruster slider after cooldown completes
    public void ResetThrusterSlider()
    {
        if (_thrusterSlider != null)
        {
            _thrusterSlider.value = 1;  // reset slider to full charge
            UpdateThrusterPercentageText(1);  // update percentage to 100%
        }
    }

    private void UpdateThrusterPercentageText(float sliderValue)
    {
        if (_thrusterPercentageText != null) // check if Text is not Inactive
        {
            int percentage = Mathf.RoundToInt(sliderValue * 100);  // convert slider value (0 to 1) to percentage (0% to 100%)
            _thrusterPercentageText.text = percentage.ToString() + "%"; // convert text to string
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
}
