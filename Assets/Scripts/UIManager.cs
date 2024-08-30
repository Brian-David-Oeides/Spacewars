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
