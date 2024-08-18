using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;

    private void Update()
    {
        // if the R key is pressed
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            // restart the current scene
            SceneManager.LoadScene(1); // current game scene
        }

        // if Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // quit application
            Application.Quit();
        }

    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
