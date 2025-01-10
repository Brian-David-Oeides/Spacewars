using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for displaying wave text

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private SpawnManager _spawnManager; // reference to the spawn manager

    private int _currentWave = 1;
    private int _enemiesToSpawn;
    private int _enemiesDestroyed = 0;
    private const int _maxWaves = 4; // maximum number of waves (4 waves)

    [SerializeField] 
    private Text _waveText; // UI Text for wave display

    private UIManager _uiManager; // Reference to UIManager for displaying wave numbers

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>(); // find the UIManager on the Canvas
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is NULL!");
        }
    }
    public void StartWave(int wave)
    {
        if (wave <= _maxWaves)
        {
            StartCoroutine(DisplayWaveText(wave));

            // call DisplayWave to show wave number
            if (_uiManager != null)
            {
                _uiManager.DisplayWave(wave); // display wave number on UI
            }

            // dynamically calculate enemies to spawn each wave
            _enemiesToSpawn = Random.Range(3 + (wave * 2), 5 + (wave * 3));

            // delegate enemy spawning to SpawnManager
            _spawnManager.StartSpawning(_enemiesToSpawn, wave); // pass enemy count and wave number
        }
        else if (wave == _maxWaves + 1)
        {
            Debug.Log("Spawning Boss...");
            _spawnManager.SpawnBoss(); // Call SpawnBoss method from SpawnManager
        }
        else
        {
            Debug.Log("All waves destroyed!");
        }

    }

    public void EnemyDestroyed()
    {
        _enemiesDestroyed++;

        if (_enemiesDestroyed >= _enemiesToSpawn)
        {
            _currentWave++;
            _enemiesDestroyed = 0; // reset counter for new wave

            // trigger next wave, if it's within max number of waves
            if (_currentWave <= _maxWaves)
            {
                StartWave(_currentWave); // start next wave when all enemies destroyed
            }
            else
            {
                StartWave(_currentWave); // Start boss wave
            }
        }
    }
    IEnumerator DisplayWaveText(int wave)
    {
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveText.gameObject.SetActive(false);
    }

}
