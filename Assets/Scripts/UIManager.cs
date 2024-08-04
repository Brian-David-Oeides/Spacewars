using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    // declare reference data type
    [SerializeField]
    private Text _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        // assign text component to the reference data type
        _scoreText.text = "Score: " + 0;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
}
