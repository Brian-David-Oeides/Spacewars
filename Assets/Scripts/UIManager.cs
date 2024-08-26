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

    // UI Text displaying ammo count
    [SerializeField]
    private Text _ammoText; 

    private GameManager _gameManager;

    //private Coroutine _flashAmmoRoutine; // Reference to manage the ammo flashing routine

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

    /*public void FlashAmmoUI()
    {
        // Check if there's already a flashing routine running, and stop it
        if (_flashAmmoRoutine != null)
        {
            StopCoroutine(_flashAmmoRoutine);
        }

        // Start a new flashing routine for the ammo UI
        _flashAmmoRoutine = StartCoroutine(FlashAmmoRoutine());
    }

    private IEnumerator FlashAmmoRoutine()
    {
        while (true)
        {
            _ammoText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            _ammoText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }
    */// new stop

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
